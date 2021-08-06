using System;
using System.Collections.Generic;
using System.Linq;
using rewind.Animation;
using rewind.Fragment;
using rewind.Rewindable.Npc.Tasks;
using Sandbox;

namespace rewind.Rewindable.Npc
{
	[Library("ent_npc_rewindable")]
	public partial class RewindableNpc : AnimEntity, IRewindable
	{
		public Stack<RewindFragment> Fragments { get; set; } = new();
		public CitizenAnimator animator { get; set; }
		public BaseTask CurrentTask { get; set; }
		
		private Vector3 inputVelocity;
		private RewindFragment lastFragment;
		private string lastSequence = null;

		public override void Spawn()
		{
			base.Spawn();

			SetModel("models/citizen/citizen.vmdl");
			animator = new(this);

			CollisionGroup = CollisionGroup.Player;
			EyePos = Position + Vector3.Up * 64;

			SetupPhysicsFromCapsule(PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius(72, 8));
			MoveType = MoveType.MOVETYPE_WALK;
			EnableHitboxes = true;

			//Outfit = new Outfit(this);
			//Outfit.ApplyOutfit();
			
			Log.Info( $"I'm alive! {IsClient}" );

			Health = 100;
		}
		
		[Event.Tick.Client]
		public void Tick()
		{
			if ( RewindGame.Mode != RewindMode.Gameplay )
				return;
			
			if (CurrentTask is not null && !CurrentTask.Completed)
			{
				inputVelocity = CurrentTask.CalculateInputVelocity();
				Velocity = Velocity.AddClamped(inputVelocity * Time.Delta * 500, CurrentTask.Speed);
			}
			else
			{
				int chance = Rand.Int(6);

				CurrentTask = chance == 1 ? 
					new WalkTask(this) :
					new RunTask(this);
			}

			Move(Time.Delta);

			animator.Tick(inputVelocity, Velocity);
		}
		
		protected virtual void Move( float timeDelta )
		{
			var bbox = BBox.FromHeightAndRadius( 64, 4 );
			var move = new MoveHelper( Position, Velocity );
			move.Trace = move.Trace.Ignore( this ).Size( bbox.Mins, bbox.Maxs );
			move.MaxStandableAngle = 65;

			move.TryUnstuck();
			move.TryMoveWithStep( timeDelta, 20 );

			var tr = move.TraceDirection( Vector3.Down * 10 );

			if ( move.IsFloor( tr ) )
			{
				GroundEntity = tr.Entity;

				if ( !tr.StartedSolid ) // Keeps the entity on the ground
				{
					move.Position = tr.EndPos;
				}

				if ( inputVelocity.Length > .5f )
				{
					Vector3 movement = move.Velocity.Dot( inputVelocity.Normal );
					move.Velocity -= movement * inputVelocity.Normal;
					move.ApplyFriction( tr.Surface.Friction * 10, timeDelta );
					move.Velocity += movement * inputVelocity.Normal;
				}
				else
				{
					move.ApplyFriction( tr.Surface.Friction * 10, timeDelta );
				}
			}
			else
			{
				move.Velocity += PhysicsWorld.Gravity * timeDelta;
				GroundEntity = null;
			}

			Position = move.Position;
			Velocity = move.Velocity;
		}
		
		public override void OnAnimEventFootstep(Vector3 position, int foot, float volume)
		{
			var tr = Trace.Ray(position, position + Vector3.Down * 20)
				.Radius(1)
				.Ignore(this)
				.Run();

			if (!tr.Hit || Host.IsServer) return;

			float stepDistance = Vector3.DistanceBetween(Input.Position, tr.EndPos);
			float stepVolume = (float)Math.Pow(1 - (stepDistance / 512), .5);

			if (stepVolume < 0)
				return;

			tr.Surface.DoFootstep(this, tr, foot, stepVolume);
		}
		
		public static void SpawnNpcs(int count, Vector3 fallbackPos)
		{
			var spawnPoints = Entity.All.OfType<SpawnPoint>().Select( x => x.Position ).ToList();

			Log.Info( $"Found {spawnPoints.Count} spawn points {Host.IsServer}" );
			Log.Info( NavMesh.IsLoaded );
			
			if (spawnPoints.Count == 0)
				spawnPoints.Add( fallbackPos );
			
			for (int i = 0; i < count; i++)
			{
				Vector3 randomSpawnPos = spawnPoints[Rand.Int(0, spawnPoints.Count - 1)];
				Vector3? potentialSpawn = NavMesh.GetPointWithinRadius(randomSpawnPos, 500, 3500);
				
				Log.Info( "ps: " + potentialSpawn );

				if (potentialSpawn is not Vector3 spawnLocation)
					return;

				SpawnOnClient( To.Everyone, spawnLocation );
			}
		}

		[ClientRpc]
		public static void SpawnOnClient( Vector3 position )
		{
			var npc = new RewindableNpc();
			npc.Position = position;
		}
		
		public void RewindTick()
		{
			var debugPos = this.GetBoneTransform( 0 ).Position;
			DebugOverlay.Text( debugPos, 0, $"Fragments: {Fragments.Count}", Color.White );
			DebugOverlay.Text( debugPos, 1, $"Task: {CurrentTask?.Name}", Color.White );
			DebugOverlay.Text( debugPos, 2, $"Sequence: {Sequence}", Color.White );
			
			if ( RewindGame.Mode == RewindMode.Gameplay )
			{
				var frag = new RewindFragment( this );
				frag.SaveBones( this );
				frag.SaveAnimator( this );

				Fragments.Push( frag );

				// TODO: Handle more fragments than MAX_FRAGMENT_COUNT
			}
			else if ( RewindGame.Mode == RewindMode.Rewind )
			{
				if ( Fragments.TryPop( out var fragment ) )
				{
					this.ApplyFragment( fragment );
					fragment.RestoreAnimator( this );

					this.lastFragment = fragment;
				}
				else
				{
					this.ApplyFragment( this.lastFragment );
					this.lastFragment.RestoreAnimator( this );
				}
			}
		}

		public void UpdateRewindState( RewindMode mode )
		{
			switch ( mode )
			{
				case RewindMode.Gameplay:
					//PhysicsEnabled = true;
					//UseAnimGraph = true;
					//Sequence = this.lastSequence;
					//PlaybackRate = 0.3f;
					break;
				case RewindMode.Rewind:
					//PhysicsEnabled = false;
					//UseAnimGraph = false;
					//PlaybackRate = 0f;
					//this.lastSequence = Sequence;
					//Sequence = null;
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof(mode), mode, null );
			}
		}

		private void ApplyFragment( RewindFragment fragment )
		{
			Position = fragment.Position;
			EyePos = fragment.EyePos;
			Rotation = fragment.Rotation;
			Velocity = fragment.Velocity;
			LocalPosition = fragment.LocalPosition;
			LocalRotation = fragment.LocalRotation;
			Sequence = fragment.Sequence;

			for ( var i = 0; i < fragment.Bones.Length; i++ )
			{
				this.SetBoneTransform( i, fragment.Bones[i] );
			}
		}
	}
}
