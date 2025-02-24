> [!WARNING]
> This packages used in own indie-game project. The package is provided as a portfolio project.

# MVVM

An extensible MVVM binding solution for Unity Development.
This package automatically creates data binding between `view model` and `view` using Reflection, attributes and reactive properties.

## Usage

### Bind ViewModel and View
1. Create `ViewModel` and `View` scripts.

```csharp
public class ViewModel : MonoBehaviour
{
        
}
```

```csharp
public class View : View<ViewModel>
{
        
}
```

2. Move Components to yours GameObjects. These components don't have to be on the same GameObject.
3. Add `MonoBehaviour Binder` to your View Component. Drag and drop references into fields;

![viewmodel](https://github.com/topchyi39/mvvm-resources/blob/main/viewmodel.png)
![view](https://github.com/topchyi39/mvvm-resources/blob/main/view.png)

### Bind Properties

For bind view model and view properties you need a several things:
1. Add `Reactive Property` to view model. Example:
```csharp
private ReactiveProperty<string> text;
```
2. Add `TMP_Text` serialized field to `view` and assign it.
```csharp
[SerializeField] private TMP_Text label;
```
3. Add attribute `Bind` to your fields. Bind Attribute requires to put unique identifier for this class.
ViewModel example:
```csharp
[Bind("text")] private ReactiveProperty<string> text = new ("");
```

> [!WARNING]
> If your `view model` field was null `Binder` create new instance for it with default values. Field will have an instance after Awake!

View example:
```csharp
[Bind("text")] [SerializeField] private TMP_Text label;
```
4. For test, we can add Start function to set value into properties and check result. Let's add a timer for a more visual result.
```csharp
private async void Start()
{
    await Task.Delay(TimeSpan.FromSeconds(2f));
    text.Value = "Hello World!";
}
```

5. Result: at first our text gets an `empty string`, because we specified it in the constructor of the reactive property field. Then after 2 seconds the text `Hello World!`

#### With Zenject

If you use `Zenject/Extenject` in your project, you can inject our `view model`. For injecting we replace `MonoBehaviourBinder` with `InjectBinder`.

With this, you don't always need to inherit from `MonoBehaviour` for the `view model`.
To work, you just need to inject our model into the Zenject container.

```csharp
public override void InstallBindings()
{
    Container.Bind<ViewModel>().AsSingle().NonLazy();
}
```

### Reactive Collections

You can use `Observable Collection<>` to bind with `ObservableCollectionView<>`.
You need create additional `view model` and `view` **children** classes. There's an example of what it should look like.
```csharp
public class ChildViewModel
{
        
}
    
public class ViewModel
{
    [Bind("collection")] private ObservableList<ChildViewModel> _data;
}
```
```csharp
public class ChildView : View<ChildViewModel>
{
        
}
    
public class View : View<ViewModel>
{
    [Bind("collection")] [SerializeField] private ObservableListView<ChildView> data;
}
```
`ObservableListView<T>` has two assignable properties:
- Prefab : T
- Container : Transform

**Prefab** is used for create instance into **Container**, when an add event is invoked from `ObservableList` in `ViewModel`.

![img](https://github.com/topchyi39/mvvm-resources/blob/main/view-observablelist.png)

Now, you need to create prefab of your `ChildView` and assign it to `ObservableListView`.

### Concrete Binders for View Model and View
You can create concrete binders for all your types. To do this, create a class that inherits from `Binder`. This is a generic type that takes the model and view types and passes them into its constructor.
With this concrete binders you haven't install `R3` package.
```csharp
//Example with R3
public class ConcreteBinder : Binder<View, ViewModel>
{
    private IDisposable _disposable;

    public ConcreteBinder(View view, ViewModel model) : base(view, model)
    {
    }

    public override void Bind()
    {
        _disposable = Model.text.Subscribe(View.label.SetText);
    }

    public override void Unbind()
    {
        _disposable?.Dispose();
    }
}
```
```csharp
//Example without R3
public class ConcreteBinder : Binder<View, ViewModel>
{
    private IDisposable _disposable;

    public ConcreteBinder(View view, ViewModel model) : base(view, model)
    {
    }

    public override void Bind()
    {
        Model.OnTextChanged += View.label.SetText;
    }

    public override void Unbind()
    {
        Model.OnTextChanged -= View.label.SetText;
    }
}
```

### Concrete Binder for fields
You can also create binders for concrete type of fields.
You have 2 ways to implement binder for it.

```csharp
//Binder with a direct reference to the viev model field type
public class TMPBinder : Binder<TMP_Text, ReactiveProperty<string>>
{
    private IDisposable _disposable;

    public TMPBinder(TMP_Text view, ReactiveProperty<string> model) : base(view, model) { }

    public override void Bind()
    {
        _disposable = Model.Subscribe(View.SetText);
    }

    public override void Unbind()
    {
        _disposable?.Dispose();
    }
}
```
This type of binder supports subscribe to ReactiveProperty and Subject 
```csharp
//Binder with generic type of Observer<>
public class TMPBinder : ObservableBinder<TMP_Text, string>
{
    private IDisposable _disposable;

    public TMPBinder(TMP_Text view, Observable<string> model) : base(view, model) { }

    public override void Bind()
    {
        _disposable = Model.Subscribe(View.SetText);
    }

    public override void Unbind()
    {
        _disposable?.Dispose();
    }
}
```



## Notes

#### Own binder `view` and `view model`.
You can create instances of the binder yourself. To do this, you just need to call `BinderFactory.Create`. It returns `IBinder` object.
For enabling binder invoke the `Bind` method. For disabling binder invoke the `Unbind` method.
```csharp
// Create binder
var binder = BinderFactory.Create(view, viewModel);
            
// Subscribe to all available fields
binder.Bind();
            
//Dispose all subscriptions
binder.Unbind();
```

### Available Binders

| Binder Name                   |      ViewType       |        ViewModel        |
|-------------------------------|:-------------------:|:-----------------------:|
| ButtonMethodBinder            |       Button        |         Method          |
| ButtonObservableBinder        |       Button        |     Subject\<Unit>      |
| EmptyMethodObservableBinder   |       Method        |     Observable\<T>      |
| GenericMethodObservableBinder |     Method\<T>      |     Observable\<T>      |
| ToggleMethodBinder            |       Toggle        |      Method\<bool>      |
| ToggleBinder                  |       Toggle        | ReactiveProperty\<bool> |
| TMPTextBinder                 |      TMP_Text       |     Observable\<T>      |
| TMPInputBinder                |      TMP_Input      |    ISubject\<string>    |
| ObservableListBinder          | ObservableListView  |     ObservableList      |


## How to Install

### Dependencies

1. Install **`R3`** from NuGet using [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity).
    - Open window from `NuGet > Manage NuGet Packages`.
    - Search **`R3`** and press "Install".
2. Install **`ObservableCollections`** and **`ObservableCollections.R3`** from NuGet, like as previous.
3. Install **`R3.Unity`** from [Git](https://github.com/Cysharp/R3#unity);

### Install via Package Manager
- Click `Window > Package Manager`.
- Click `+ > Add package from git URL` and input the repository URL:
````
https://github.com/topchyi39/mvvm.git
````

### Manually Install

- Open the `Packages/manifest.json` file in your project. Then add this package somewhere in the dependencies block:
````
"com.topchyi39.mvvm": "https://github.com/topchyi39/mvvm.git"
````
