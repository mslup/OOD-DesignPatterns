# Database behaviour simulator 

Half-semester-long project for the Object Oriented Design course at the MiNI WUT faculty. Its goal was to create a command-line interface allowing access and modification of a set of collections of related objects. 

## Design patterns
Some of the used OOD patterns are:
- builder,
- factory,
- command,
- memento,
- adapter,
- iterator.

## Implemented commands
- `list`
- `find`
- `add`
- `edit`
- `delete`
- `exit`

The program stores the history of executed commands and provides `undo` and `redo` functionality. A prior version of the project implemented a command queue, which delayed command execution until an appropriate commit request was called. 

Additionally, the command history can be exported into and imported from XML or plaintext formats.
