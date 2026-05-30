# RPG Console Game – Object-Oriented Design Project

A console-based RPG game written in C# as part of an Object-Oriented Design course.

The project focuses on creating a simple RPG game while applying object-oriented programming principles and design patterns. The game is developed in stages, where each stage adds new functionality and improves the architecture of the program.

## Project Description

The player explores a dungeon, moves around the map, collects items and weapons, manages equipment, fights enemies, and interacts with the game world through keyboard commands.

The project was designed to make the code easy to extend. Instead of putting all logic in one place, responsibilities are separated between classes. Interfaces and polymorphism are used to avoid large conditional blocks and to make it easier to add new elements, such as new attack styles, enemies, dungeon generation steps, themes, or item modifiers.

## Design Patterns

### Builder Pattern

The Builder pattern was used for dungeon generation.

The dungeon is created step by step. Separate generation steps are responsible for creating different parts of the dungeon, such as the initial map, rooms, paths, items, weapons, currency, and enemies.

This makes the dungeon creation process more flexible, because new generation steps can be added without changing the whole dungeon-building logic.

### Strategy Pattern

The Strategy pattern was used in places where the program needs to choose between different algorithms or behaviors.

It was used for attack styles in the combat system. Each attack style has its own way of calculating combat values, so the combat logic can use different strategies without knowing the exact details of each one.

It was also used for dungeon generation strategies and themes. Different themes can create different dungeon-generation behavior, which makes the game easier to extend with new variants.

### Decorator Pattern

The Decorator pattern was used for item and weapon modifiers.

Modifiers can change the behavior or statistics of an item or weapon without creating many separate subclasses for every possible item variation.

This allows weapons and items to receive additional effects while keeping the base item classes simple.

### Singleton Pattern

The Singleton pattern was used in the event logging system.

The game has one shared access point to the current event log. Thanks to this, different modules of the project can add log entries without directly creating or passing the logger everywhere.

This is used for logging actions such as movement, combat, equipping items, picking up objects, enemy reactions, and important game events.

### Factory Pattern

The Factory pattern was used to separate object creation from the main game logic.

It is useful in places where the program needs to create objects depending on the selected configuration or theme. This keeps the code cleaner, because the main game logic does not need to know all details of how specific objects are created.

### Observer Pattern

The Observer pattern was used for reactive enemies.

Enemies can react to events happening in the game, such as noise created by the player or the death of another enemy from the same species.

For example, when an enemy dies, other enemies of the same species can be notified and change their statistics. When the player creates noise, enemies decide whether they hear it and how they should react.

This keeps enemy behavior loosely coupled from the player and item logic. The player does not directly control enemy reactions; it only creates an event, and enemies react to it themselves.

## Event Logging

The project includes an event logging system that saves important gameplay events to a file.

The logger stores the full history of events in a log file. It also keeps a limited number of recent entries in memory, so they can be displayed in the game sidebar.

Logged events include player actions, combat results, equipment changes, enemy reactions, selected theme, and other important events during gameplay.

## Themes

The game supports different dungeon themes.

A theme can define the general style of the dungeon and influence the way the game is generated. This makes the game more flexible and allows new themes to be added without rewriting the main game logic.

## Reactive Enemies

Enemies are grouped by species and can react to specific events.

When an enemy dies, other enemies from the same species are notified. Depending on the species, they can become weaker, stronger, or react in another way.

The game also supports sound events. Some player actions can create noise. The noise travels through walkable parts of the dungeon, and enemies decide whether they are close enough to hear it.

This part of the project demonstrates loose coupling and event-based communication between objects.