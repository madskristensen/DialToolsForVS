using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DialControllerTools.Helpers;
using Microsoft.VisualStudio.Threading;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;
//Timer idea from https://stackoverflow.com/questions/26020799/enforcing-a-delay-on-textbox-textchanged
namespace DialControllerTools
{
    public partial class CustomOptionsControl : UserControl
    {
        private string _selectedText;
        private Timer _timer;
        private readonly string commandsString;

        private ImmutableArray<string> commands;
        private ImmutableArray<string> Commands
         => commands.IsDefaultOrEmpty
            ? commands = VsCommands.ParseCommands(commandsString)
            : commands;

        private CustomOptions customOptions;
        internal CustomOptions CustomOptions
        {
            get => customOptions;
            set
            {
                customOptions = value;
                AssignedClickLabel.Text = customOptions.ClickAction;
                AssignedRightLabel.Text = customOptions.RightAction;
                AssignedLeftLabel.Text = customOptions.LeftAction;
            }
        }

        public CustomOptionsControl()
        {
            InitializeComponent();
            CommandsBox.Text = commandsString = VsCommands.ReadCommandsAsString();
            VsCommands.CheckEmptyEntries(commandsString);

            _timer = new Timer();
            _timer.Interval = 300;
            _timer.Tick += Timer_Tick;
        }

        private void AssignClickAction_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedText))
            {
                CustomOptions.ClickAction = _selectedText;
                AssignedClickLabel.Text = _selectedText;
            }
        }

        private void AssignRightAction_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedText))
            {
                CustomOptions.RightAction = _selectedText;
                AssignedRightLabel.Text = _selectedText;
            }
        }

        private void AssignLeftAction_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedText))
            {
                CustomOptions.LeftAction = _selectedText;
                AssignedLeftLabel.Text = _selectedText;
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.Tag = SearchBox.Text;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var searchText = _timer.Tag?.ToString() ?? string.Empty;
            var results = Commands.Where(c => c.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) > -1);
            int newLineSymbolsLength = Environment.NewLine.Length;
            CommandsBox.Text = results.Any()
                ? results.Aggregate(
                    new StringBuilder(results.Sum(r => r.Length + newLineSymbolsLength)),
                    (accum, item) =>
                    {
                        accum.Append(item);
                        accum.Append(Environment.NewLine);
                        return accum;
                    },
                    accum =>
                    {
                        accum.Length -= newLineSymbolsLength;
                        return accum.ToString();
                    })
                : string.Empty;
        }

        private void CommandsBox_MouseClick(object sender, MouseEventArgs e)
        {
            var current_line = CommandsBox.GetLineFromCharIndex(CommandsBox.SelectionStart);
            _selectedText = CommandsBox.Lines[current_line];
            var line_length = _selectedText?.Length ?? 0;
            CommandsBox.SelectionStart = CommandsBox.GetFirstCharIndexOfCurrentLine();
            CommandsBox.SelectionLength = line_length;
        }
    }
}
