# Strict XAML

XAML controls that follow strict guidelines to fix common issues.

## Strict Combo Box

OK, so there's only one control in this library. But it fixes a pretty common problem.

When you data bind to both `ItemsSource` and `SelectedItem`, the default control will do something strange.
When the `ItemsSource` changes so that the `SelectedItem` is no longer in the list, it will set `SelectedItem` to null.
This even happens if you are just about to change `SelectedItem` to an item in the list.
Furthermore, even if you check for null in your view model's property setter, the dependency property and your view model will simply disagree.
You will be left with a combo box having nothing selected.
There is no way to fix this issue in the view model itself.

The strict combo box follows this strict guideline:

* All input was initiated by the user.
* All output was initiated by the application.

The corollary being that the control itself cannot initiate property changes.
In particular, it cannot set `SelectedItem` to null.

### Using the StrictComboBox

Either copy the source code or install the NuGet package.

```
Install-Package StrictXAML
```

Add the strict namespace. Then bind to the `StrictSelectedItem` property instead of `SelectedItem`.

```xaml
<Window x:Class="MyProject.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:strict="clr-namespace:StrictXAML;assembly=StrictXAML"
        Title="MainWindow">
    <Grid>
        <strict:StrictComboBox ItemsSource="{Binding Options}" StrictSelectedItem="{Binding SelectedOption}" />
    </Grid>
</Window>
```

All of the other base class properties are still available, including `SelectedItem`.

You can drop the guard clause from your property setter.