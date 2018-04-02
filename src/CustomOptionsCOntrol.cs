using System;
using System.Linq;
using System.Windows.Forms;
using DialToolsForVS.Helpers;
using Microsoft.VisualStudio.Threading;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;
//Timer idea from https://stackoverflow.com/questions/26020799/enforcing-a-delay-on-textbox-textchanged
namespace DialToolsForVS
{
    public partial class CustomOptionsControl : UserControl
    {
        private string _selectedText;
        private Timer _timer;
        internal CustomOptions CustomOptions;

        public CustomOptionsControl()
        {
            InitializeComponent();
            _timer = new Timer();
            _timer.Interval = 300;
            _timer.Tick += Timer_Tick;
        }

        public void Initialize()
        {
            CommandsBox.Text = VsCommands.CommandsAsString;
            AssignedClickLabel.Text = CustomOptions.ClickAction;
            AssignedRightLabel.Text = CustomOptions.RightAction;
            AssignedLeftLabel.Text = CustomOptions.LeftAction;
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
            var searchText = _timer.Tag?.ToString().ToLower() ?? string.Empty;
            var results = VsCommands.Commands.Where(_ => _.ToLower().Contains(searchText));
            CommandsBox.Text = results.Any() ? results.Aggregate((a, b) => $"{a ?? string.Empty}{Environment.NewLine}{b ?? string.Empty}") : string.Empty;
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
