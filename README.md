# YouTubeCLI

A command line interface for building and managing YouTube broadcasts.

## Index <!-- omit in toc -->

- [TODOS](#todos)
- [Compile](#compile)
- [Command line syntax examples](#command-line-syntax-examples)

## TODOS
- [ ] More documentation
- [ ] Ability to manage broadcasts for multiple channels
- [ ] Need to find a way to manage multiple channels well

## Compile

Build `exe`:
`dotnet publish -r win-x64 -p:PublishSingleFile=True --self--contained false`

## Command line syntax examples

- Top level help - `ytc -h`

```bash
Usage: ytc [command] [options]

Options:
  -v|--version    Show version information.
  -h|--help       Show help information.
  -t|--test-mode  Create the first active broadcast with private visibility.

Commands:
  create          Create YouTube Broadcasts
  list            List YouTube Broadcasts
  update          Update Broadcast
```

- Sample test mode - `-t`

  Creates one private broadcast from one active broadcast definitions.

```bash
ytc create -t --youtube-user [user] --file "[broadcasts.json]" --client-secrets "[client_secrets.json]" --output MonthlyCsv --occurences 4
```

- Sample creation from broadcast file
```bash
ytc create --youtube-user [user] --client-secrets "[client_secrets.json]" ---output DailyCsv --occurences 2 --file "[broadcast.json]"
```

- Sample update single broadcast

```bash
ytc update --youtube-user [user]  --client-secrets "[client_secrets.json]" --youtube-user [user] -p private --auto-start=false --auto-stop=true
```

- Sample update batch from csv file

```bash
ytc update --youtube-user [user]  --client-secrets "[client_secrets.json]" --file "[broadcasts_info.csv]"
```
