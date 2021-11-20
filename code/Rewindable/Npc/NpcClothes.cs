﻿using Sandbox;

namespace rewind.Rewindable.Npc
{
	public partial class RewindableNpc
	{
		private ModelEntity pants;
		private ModelEntity jacket;
		private ModelEntity shoes;
		private ModelEntity hat;

		private bool dressed;

		private Clothing.Container container = new();

		private void DressUp()
		{
			if ( this.dressed )
			{
				return;
			}

			this.dressed = true;

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
					"models/citizen_clothes/trousers/trousers.jeans.vmdl",
					"models/citizen_clothes/trousers/trousers.lab.vmdl",
					"models/citizen_clothes/trousers/trousers.police.vmdl",
					"models/citizen_clothes/trousers/trousers.smart.vmdl",
					"models/citizen_clothes/trousers/trousers.smarttan.vmdl",
					"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl",
					"models/citizen_clothes/trousers/trousers_tracksuit.vmdl",
					"models/citizen_clothes/shoes/shorts.cargo.vmdl"
				} );

				this.pants = new ModelEntity();
				this.pants.SetModel( model );
				this.pants.SetParent( this, true );
				this.pants.EnableShadowInFirstPerson = true;
				this.pants.EnableHideInFirstPerson = true;

				this.SetBodyGroup( "Legs", 1 );
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
					//"models/citizen_clothes/jacket/labcoat.vmdl",
					"models/citizen_clothes/jacket/jacket.red.vmdl",
					"models/citizen_clothes/jacket/jacket.tuxedo.vmdl",
					"models/citizen_clothes/jacket/jacket_heavy.vmdl"
				} );

				this.jacket = new ModelEntity();
				this.jacket.SetModel( model );
				this.jacket.SetParent( this, true );
				this.jacket.EnableShadowInFirstPerson = true;
				this.jacket.EnableHideInFirstPerson = true;

				var propInfo = this.jacket.GetModel().GetPropData();
				if ( propInfo.ParentBodygroupName != null )
				{
					this.SetBodyGroup( propInfo.ParentBodygroupValue, propInfo.ParentBodygroupValue );
				}
				else
				{
					this.SetBodyGroup( "Chest", 0 );
				}
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
					"models/citizen_clothes/shoes/trainers.vmdl",
					"models/citizen_clothes/shoes/shoes.workboots.vmdl"
				} );

				this.shoes = new ModelEntity();
				this.shoes.SetModel( model );
				this.shoes.SetParent( this, true );
				this.shoes.EnableShadowInFirstPerson = true;
				this.shoes.EnableHideInFirstPerson = true;

				this.SetBodyGroup( "Feet", 1 );
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
					"models/citizen_clothes/hat/hat_hardhat.vmdl", "models/citizen_clothes/hat/hat_woolly.vmdl",
					"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
					"models/citizen_clothes/hair/hair_malestyle02.vmdl",
					"models/citizen_clothes/hair/hair_femalebun.black.vmdl",
					"models/citizen_clothes/hat/hat_beret.red.vmdl", "models/citizen_clothes/hat/hat.tophat.vmdl",
					"models/citizen_clothes/hat/hat_beret.black.vmdl", "models/citizen_clothes/hat/hat_cap.vmdl",
					"models/citizen_clothes/hat/hat_leathercap.vmdl",
					"models/citizen_clothes/hat/hat_leathercapnobadge.vmdl",
					"models/citizen_clothes/hat/hat_securityhelmetnostrap.vmdl",
					"models/citizen_clothes/hat/hat_service.vmdl",
					"models/citizen_clothes/hat/hat_uniform.police.vmdl",
					"models/citizen_clothes/hat/hat_woollybobble.vmdl"
				} );

				this.hat = new ModelEntity();
				this.hat.SetModel( model );
				this.hat.SetParent( this, true );
				this.hat.EnableShadowInFirstPerson = true;
				this.hat.EnableHideInFirstPerson = true;
			}
		}
	}
}
