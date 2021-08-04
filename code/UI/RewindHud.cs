using Sandbox.UI;
using Sandbox.UI.Construct;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace rewind.UI
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class RewindHudEntity : Sandbox.HudEntity<RootPanel>
	{
		public RewindHudEntity()
		{
			if ( IsClient )
			{
				RootPanel.SetTemplate( "/ui/rewindhud.html" );
				RootPanel.StyleSheet.Load( "/ui/rewindhud.scss" );

				var tutorial = RootPanel.Add.Panel("infocontainer");
				tutorial.Add.Label( "Left click to shoot box" );
				tutorial.Add.Label( "Right click to rewind" );
				tutorial.Add.Label( "F to shoot ragdoll" );
			}
		}
	}

}
