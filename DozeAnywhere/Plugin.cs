using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using FFXIVClientStructs.FFXIV.Client.UI.Misc;

using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;
using HotbarSlotType = FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureHotbarModule.HotbarSlotType;

namespace DozeAnywhere;

public sealed class Plugin : IDalamudPlugin
{
	private const int HorbarSize = 12;
	private const int HotbarsNum = 18;

	[PluginService]
	public static IClientState ClientState { get; private set; } = null!;

	[PluginService]
	public static ICommandManager CommandManager { get; private set; } = null!;

	[PluginService]
	public static IChatGui ChatGui { get; private set; } = null!;

	[PluginService]
	public static IKeyState KeyState { get; private set; } = null!;

	[PluginService]
	public static IObjectTable ObjectTable { get; private set; } = null!;

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
				99,
				"Use default /doze emote and «doze anywhere» if ALT is pressed."));

		CommandManager.AddHandler(
			"/createicons",
			new CommandInfo((_, args) => CreateRegularIcons(args.Split(" ")))
			{
				HelpMessage = "Creates an action button on specified hotbar that works «standalone». Use this command without arguments for more information.",
			});
	}

	private unsafe void CreateRegularIcons(string[] args)
	{
		if (args.Length != 3)
		{
			ChatGui.PrintError("Use:");
			ChatGui.PrintError("/createicons sit 1 5 — create «sit everywhere» action on hotbar 1 slot 5.");
			ChatGui.PrintError("/createicons doze 1 5 — create «sit everywhere» action on hotbar 1 slot 5.");
			return;
		}

		uint commandId;
		uint effectiveHotbarNumber;
		uint effectiveSlotPosition;

		switch (args[0])
		{
			case "sit":
				commandId = 96;
				break;
			case "doze":
				commandId = 99;
				break;
			default:
				ChatGui.PrintError("First argument should be either «sit» or «doze»!");
				return;
		}

		if (!uint.TryParse(args[1], out var hotbarNumber) || hotbarNumber < 1 || hotbarNumber > HotbarsNum)
		{
			ChatGui.PrintError($"Second argument should be number between 1 and {HotbarsNum}!");
			return;
		}
		else
		{
			effectiveHotbarNumber = hotbarNumber - 1;
		}

		
		if (!uint.TryParse(args[2], out var slotPosition) || slotPosition < 1 || slotPosition > HorbarSize)
		{
			ChatGui.PrintError($"Third argument should be number between 1 and {HorbarSize}!");
			return;
		}
		else
		{
			effectiveSlotPosition = slotPosition - 1;
		}

		var hotbarSlot = RaptureHotbarModule->GetSlotById(
			effectiveHotbarNumber,
			effectiveSlotPosition);

		hotbarSlot->Set(HotbarSlotType.Emote, commandId);

		RaptureHotbarModule->WriteSavedSlot(
			RaptureHotbarModule->ActiveHotbarClassJobId,
			effectiveHotbarNumber,
			effectiveSlotPosition,
			hotbarSlot,
			false,
			ClientState.IsPvP);
	}

	private unsafe CommandInfo GetExecuteEmotionCommand(uint baseCommandId, uint innerCommandId, string commandHelpMessage)
	{
		return new CommandInfo((command, args) =>
		{
			if (ObjectTable.LocalPlayer is null || ObjectTable.LocalPlayer.IsDead)
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
