- Put all serialized "global" variables in a GameData scriptable object that is accessed via a GameManager singleton.

## Packages

- Use the Krooq Core and Krooq Common packages for common functionality (getting cached component refs and singleton refs, creating objects using singleton MultiGameObjectPool, GameEvents)
- Use the Shapes package for all primitive shapes (circles, rectangles, lines, etc.)
- Use the Cinemachine 3 package for all camera control and movement.
- Use UniTask for all async/await functionality.

## Coding Conventions

- Use var always.
- Methods are protected by default unless they need to be public.
- Properties are protected by default but can have public getters as needed, setters should instead be methods if the class is a MonoBehaviour.
- Properties should be in PascalCase.
- Fields should be private by default with [SerializeField, ReadOnly] so values can be seen in the inspector.
- Fields should be in \_camelCase as they are always private.
- Use scriptable objects for edit-time data containers where possible.
- Assume there is a single prefab per object type that is configured with its required data in an "Init" function (you may pass the relevant scriptable object as a parameter to the Init function).
- Components in child objects in prefabs will be assigned in editor as [SerializeField] private fields.
- Keep code as minimal as possible, do not add functions when you can inline a simple bit of code.
- Code should be kept in feature folders.

## Game Particulars

- The game is a 2D game using orthographic projection
- Audio should play from a single stereo audio source via an AudioManager singleton.
- There is nothing special about singletons, they are just regular MonoBehaviours.
- Spawning and Despawning of objects should be done via MultiGameObjectPool singletons through a manager singleton.
