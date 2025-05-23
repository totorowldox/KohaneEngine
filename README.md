﻿# Kohane Engine

## Overview

Kohane Engine (the Engine) is a Unity-based Visual Novel (VN) engine, dedicated to the memory of Ohitome Kohane from Project KV.

The Engine aspires to be a minimalistic yet customizable VN engine, offering the core features necessary for Visual Novel development, 
just like Kohane, embodying simplicity and purity.

It leverages a simple Inversion of Control (IoC) pattern to enhance modularity and maintainability.

## Key Features

- **Customizability**: Designed to be highly customizable, allowing developers to tailor the engine to their specific needs.
- **Simplicity**: Aims to provide the essential functionality required for creating Visual Novels, without unnecessary complexity.

## Architecture
```mermaid
flowchart TD
	DependencyResolver & StoryReader --> Engine
	Serializer --> StoryReader
  Engine{Engine} --Runtime Struct--> StoryManager
  StateManager  --Decide--> StoryManager & InteractManager
  InteractManager --User Input/Auto/Skip--> StoryManager
  StoryManager --A single block--> StoryResolver
  subgraph Resolver
	StoryResolver --Distribute--> Audio & Background & Character & Text & Etc & ...
	end
	subgraph RD[Resolver dependency]
	direction LR
	DT[(DOTween)] --> Animator
  Binder & Animator & ResourceManager
  end
  RD --> Resolver
```

## Reminder
*Make sure to check `Load In Background` for any audios to load them asynchronously.*
