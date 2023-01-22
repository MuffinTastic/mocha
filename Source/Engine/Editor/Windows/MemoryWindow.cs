//using Mocha.Glue;

//namespace Mocha.Editor;

//[Title( "Memory" )]
//public class MemoryWindow : EditorWindow
//{
//	public override void Draw()
//	{
//		using ( ImGuiX.Scope windowScope = ImGuiX.Begin( "Memory" ) )
//		{
//			if ( !windowScope.Visible )
//				return;

//			using ( ImGuiX.Scope childScope = ImGuiX.BeginChild( "##memory", -1, -1 ) )
//			{
//				if ( !childScope.Visible )
//					return;

//				using ( ImGuiX.Scope tableScope = ImGui.BeginTable( "##allocations_table", 4, 0 )
//				{
//					if ( !tableScope.Visible )
//						return;

//					ImGui.TableSetupStretchColumn( "Name" );
//					ImGui.TableSetupFixedColumn( "Allocated", 32.0f );
//					ImGui.TableSetupFixedColumn( "Freed", 32.0f );
//					ImGui.TableSetupFixedColumn( "Dangling", 32.0f );
//					ImGui.TableHeaders();

//					foreach ( var item in MemoryLogger.Entries.ToList().OrderBy( x => -x.Value.Allocations ) )
//					{
//						var (allocated, freed) = item.Value;
//						var dangling = (allocated - freed);

//						ImGui.TableNextRow();
//						ImGui.TableNextColumn();

//						ImGui.Text( item.Key );

//						ImGui.TableNextColumn();

//						ImGui.Text( allocated.ToString() );

//						ImGui.TableNextColumn();

//						ImGui.Text( freed.ToString() );

//						ImGui.TableNextColumn();

//						ImGui.Text( dangling.ToString() );
//					}
//				}
//			}
//		}
//	}
//}
