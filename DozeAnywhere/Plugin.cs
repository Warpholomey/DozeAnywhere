using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using FFXIVClientStructs.FFXIV.Client.UI.Misc;

using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace DozeAnywhere;

public sealed class Plugin : IDalamudPlugin
{
	[PluginService]
	public static IClientState ClientState { get; private set; } = null!;

	[PluginService]
	public static ICommandManager CommandManager { get; private set; } = null!;

	[PluginService]
	public static IChatGui ChatGui { get; private set; } = null!;

	[PluginService]
	public static IKeyState KeyState { get; private set; } = null!;

	private unsafe RaptureHotbarModule* RaptureHotbarModule => Framework.Instance()
		->UIModule
		->GetRaptureHotbarModule();

	public Plugin()
	{
		CommandManager.AddHandler(
			"/xsit",
			GetExecuteEmotionCommand(
				50,
				96,
				"Use default /sit emote and «sit anywhere» if ALT is pressed."));

		CommandManager.AddHandler(
			"/xdoze",
			GetExecuteEmotionCommand(
				13,
				88,
				"Use default /doze emote and «doze anywhere» if ALT is pressed."));
	}

	private unsafe CommandInfo GetExecuteEmotionCommand(uint baseCommandId, uint innerCommandId, string commandHelpMessage)
	{
		return new CommandInfo((command, args) =>
		{
			if (ClientState.LocalPlayer is null || ClientState.LocalPlayer.IsDead)
			{
				return;
			}

			var effectiveCommandId = KeyState[VirtualKey.MENU]
				? innerCommandId
				: baseCommandId;

			RaptureHotbarModule->ScratchSlot.Set(HotbarSlotType.Emote, effectiveCommandId);
			RaptureHotbarModule->ExecuteSlot(&RaptureHotbarModule->ScratchSlot);
			RaptureHotbarModule->ScratchSlot.Set(HotbarSlotType.Empty, 0);
		})
		{
			HelpMessage = commandHelpMessage,
		};
	}

	public void Dispose()
	{
	}
}
