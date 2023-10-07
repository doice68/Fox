using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
using RegistryOptions = TextMateSharp.Grammars.RegistryOptions;
using Avalonia.Input;

namespace Fox.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        
        if (Design.IsDesignMode) 
            return;
        
        consoleScrollViewer.PropertyChanged += (s, e) => 
        {
            if (e.Property == ScrollViewer.ScrollBarMaximumProperty)
                consoleScrollViewer.ScrollToEnd();
        };

        textEditor.TextArea.TextEntered += TextEditor_TextChanged;

        var registryOptions = new RegistryOptions(ThemeName.Monokai);
        var textMateInstallation = textEditor.InstallTextMate(registryOptions);
        textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".js").Id));
    }

    void TextEditor_TextChanged(object? sender, TextInputEventArgs e)
    {
        if (e.Text is not ("{" or "(" or "[")) 
            return;

        switch (e.Text)
        {
            case "{":
                {
                    textEditor.Document.Insert(textEditor.TextArea.Caret.Offset, "}");
                    textEditor.TextArea.Caret.Offset -= 1;
                    break;
                }
            case "(":
                {
                    textEditor.Document.Insert(textEditor.TextArea.Caret.Offset, ")");
                    textEditor.TextArea.Caret.Offset -= 1;
                    break;
                }
            case "[":
                {
                    textEditor.Document.Insert(textEditor.TextArea.Caret.Offset, "]");
                    textEditor.TextArea.Caret.Offset -= 1;
                    break;
                }
        }
    }
}