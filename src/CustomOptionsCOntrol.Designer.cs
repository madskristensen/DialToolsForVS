namespace DialToolsForVS
{
    partial class CustomOptionsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AssignedClickLabel = new System.Windows.Forms.Label();
            this.AssignedRightLabel = new System.Windows.Forms.Label();
            this.AssignedLeftLabel = new System.Windows.Forms.Label();
            this.AssignLeftAction = new System.Windows.Forms.Button();
            this.AssignRightAction = new System.Windows.Forms.Button();
            this.AssignClickAction = new System.Windows.Forms.Button();
            this.LeftLabel = new System.Windows.Forms.Label();
            this.RightLabel = new System.Windows.Forms.Label();
            this.ClickLabel = new System.Windows.Forms.Label();
            this.CommandsBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SearchBox = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // AssignedClickLabel
            // 
            this.AssignedClickLabel.AutoEllipsis = true;
            this.AssignedClickLabel.Location = new System.Drawing.Point(211, 369);
            this.AssignedClickLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AssignedClickLabel.MaximumSize = new System.Drawing.Size(300, 25);
            this.AssignedClickLabel.Name = "AssignedClickLabel";
            this.AssignedClickLabel.Size = new System.Drawing.Size(300, 25);
            this.AssignedClickLabel.TabIndex = 26;
            // 
            // AssignedRightLabel
            // 
            this.AssignedRightLabel.AutoEllipsis = true;
            this.AssignedRightLabel.Location = new System.Drawing.Point(211, 421);
            this.AssignedRightLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AssignedRightLabel.MaximumSize = new System.Drawing.Size(300, 25);
            this.AssignedRightLabel.Name = "AssignedRightLabel";
            this.AssignedRightLabel.Size = new System.Drawing.Size(300, 25);
            this.AssignedRightLabel.TabIndex = 25;
            // 
            // AssignedLeftLabel
            // 
            this.AssignedLeftLabel.AutoEllipsis = true;
            this.AssignedLeftLabel.Location = new System.Drawing.Point(211, 471);
            this.AssignedLeftLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AssignedLeftLabel.MaximumSize = new System.Drawing.Size(300, 25);
            this.AssignedLeftLabel.Name = "AssignedLeftLabel";
            this.AssignedLeftLabel.Size = new System.Drawing.Size(300, 25);
            this.AssignedLeftLabel.TabIndex = 24;
            // 
            // AssignLeftAction
            // 
            this.AssignLeftAction.Location = new System.Drawing.Point(529, 462);
            this.AssignLeftAction.Margin = new System.Windows.Forms.Padding(4);
            this.AssignLeftAction.Name = "AssignLeftAction";
            this.AssignLeftAction.Size = new System.Drawing.Size(100, 42);
            this.AssignLeftAction.TabIndex = 23;
            this.AssignLeftAction.Text = "Assign";
            this.AssignLeftAction.UseVisualStyleBackColor = true;
            this.AssignLeftAction.Click += new System.EventHandler(this.AssignLeftAction_Click);
            // 
            // AssignRightAction
            // 
            this.AssignRightAction.Location = new System.Drawing.Point(527, 412);
            this.AssignRightAction.Margin = new System.Windows.Forms.Padding(4);
            this.AssignRightAction.Name = "AssignRightAction";
            this.AssignRightAction.Size = new System.Drawing.Size(100, 42);
            this.AssignRightAction.TabIndex = 22;
            this.AssignRightAction.Text = "Assign";
            this.AssignRightAction.UseVisualStyleBackColor = true;
            this.AssignRightAction.Click += new System.EventHandler(this.AssignRightAction_Click);
            // 
            // AssignClickAction
            // 
            this.AssignClickAction.Location = new System.Drawing.Point(527, 361);
            this.AssignClickAction.Margin = new System.Windows.Forms.Padding(4);
            this.AssignClickAction.Name = "AssignClickAction";
            this.AssignClickAction.Size = new System.Drawing.Size(100, 42);
            this.AssignClickAction.TabIndex = 21;
            this.AssignClickAction.Text = "Assign";
            this.AssignClickAction.UseVisualStyleBackColor = true;
            this.AssignClickAction.Click += new System.EventHandler(this.AssignClickAction_Click);
            // 
            // LeftLabel
            // 
            this.LeftLabel.AutoSize = true;
            this.LeftLabel.Location = new System.Drawing.Point(22, 471);
            this.LeftLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LeftLabel.Name = "LeftLabel";
            this.LeftLabel.Size = new System.Drawing.Size(154, 25);
            this.LeftLabel.TabIndex = 20;
            this.LeftLabel.Text = "Dial left action:";
            // 
            // RightLabel
            // 
            this.RightLabel.AutoSize = true;
            this.RightLabel.Location = new System.Drawing.Point(22, 421);
            this.RightLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.RightLabel.Name = "RightLabel";
            this.RightLabel.Size = new System.Drawing.Size(167, 25);
            this.RightLabel.TabIndex = 19;
            this.RightLabel.Text = "Dial right action:";
            // 
            // ClickLabel
            // 
            this.ClickLabel.AutoSize = true;
            this.ClickLabel.Location = new System.Drawing.Point(22, 369);
            this.ClickLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ClickLabel.Name = "ClickLabel";
            this.ClickLabel.Size = new System.Drawing.Size(192, 25);
            this.ClickLabel.TabIndex = 18;
            this.ClickLabel.Text = "Dial clicked action:";
            // 
            // CommandsBox
            // 
            this.CommandsBox.Location = new System.Drawing.Point(27, 97);
            this.CommandsBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CommandsBox.MaximumSize = new System.Drawing.Size(600, 238);
            this.CommandsBox.Multiline = true;
            this.CommandsBox.Name = "CommandsBox";
            this.CommandsBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.CommandsBox.Size = new System.Drawing.Size(600, 238);
            this.CommandsBox.TabIndex = 17;
            this.CommandsBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CommandsBox_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(332, 25);
            this.label1.TabIndex = 16;
            this.label1.Text = "Search for commands containing:";
            // 
            // SearchBox
            // 
            this.SearchBox.Location = new System.Drawing.Point(27, 47);
            this.SearchBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SearchBox.Name = "SearchBox";
            this.SearchBox.Size = new System.Drawing.Size(600, 31);
            this.SearchBox.TabIndex = 15;
            this.SearchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // CustomOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.AssignedClickLabel);
            this.Controls.Add(this.AssignedRightLabel);
            this.Controls.Add(this.AssignedLeftLabel);
            this.Controls.Add(this.AssignLeftAction);
            this.Controls.Add(this.AssignRightAction);
            this.Controls.Add(this.AssignClickAction);
            this.Controls.Add(this.LeftLabel);
            this.Controls.Add(this.RightLabel);
            this.Controls.Add(this.ClickLabel);
            this.Controls.Add(this.CommandsBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SearchBox);
            this.Name = "CustomOptionsControl";
            this.Size = new System.Drawing.Size(680, 600);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AssignedClickLabel;
        private System.Windows.Forms.Label AssignedRightLabel;
        private System.Windows.Forms.Label AssignedLeftLabel;
        private System.Windows.Forms.Button AssignLeftAction;
        private System.Windows.Forms.Button AssignRightAction;
        private System.Windows.Forms.Button AssignClickAction;
        private System.Windows.Forms.Label LeftLabel;
        private System.Windows.Forms.Label RightLabel;
        private System.Windows.Forms.Label ClickLabel;
        private System.Windows.Forms.TextBox CommandsBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SearchBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
