# DozeAnywhere

A plugin for FFXIV that allows you to sit or doze anywhere. This can be achieved without plugins by using a HEX editor, but some people may find it more convenient.

## Plugin's Repository

`https://raw.githubusercontent.com/Warpholomey/DalamudPlugins/master/repository.json`

## Disclaimer

**Use at your own risk. Using this plugin in public areas may result in in-game penalties.**

## Macros

This macro will use default /sit emote and «sit anywhere» if ALT is pressed:

```
/micon "Sit" emote
/xsit
```

This macro default /doze emote and «doze anywhere» if ALT is pressed:

```
/micon "Doze" emote
/xdoze
```

## Standalone

You can also create «standalone» actions that will continue to work even if the plugin is removed:

```
/createicons [sit/doze] [1-18] [1-12]
```

The second and third arguments determines in which hotbar and slot new action button should be created.
