using System;
using System.Linq;
using System.Windows.Forms;
using DialToolsForVS.Helpers;

namespace DialToolsForVS
{
    public partial class CustomOptionsControl : UserControl
    {
        private string _selectedText;

        internal CustomOptions CustomOptions;

        public CustomOptionsControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            VsCommands.Initialize();
            CommandsBox.Text = VsCommands.Commands.Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");
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

        private void SearchBox_TextChanged(object sender, System.EventArgs e)
        {
            var results = VsCommands.Commands.Where(_ => _.ToLower().Contains(SearchBox.Text?.ToLower()));
            CommandsBox.Text = results.Any() ? results.Aggregate((a, b) => $"{a??string.Empty}{Environment.NewLine}{b??string.Empty}") : string.Empty;
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
