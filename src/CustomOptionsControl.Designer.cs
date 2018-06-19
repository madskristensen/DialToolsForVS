namespace DialControllerTools
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
            this.SearcBoxLabel = new System.Windows.Forms.Label();
            this.SearchBox = new System.Windows.Forms.TextBox();
            this.SearchBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.RootLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.RootLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AssignedClickLabel
            // 
            this.AssignedClickLabel.AutoEllipsis = true;
            this.AssignedClickLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AssignedClickLabel.Location = new System.Drawing.Point(204, 450);
            this.AssignedClickLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AssignedClickLabel.Name = "AssignedClickLabel";
            this.AssignedClickLabel.Size = new System.Drawing.Size(364, 50);
            this.AssignedClickLabel.TabIndex = 26;
            this.AssignedClickLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AssignedRightLabel
            // 
            this.AssignedRightLabel.AutoEllipsis = true;
            this.AssignedRightLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AssignedRightLabel.Location = new System.Drawing.Point(204, 500);
            this.AssignedRightLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AssignedRightLabel.Name = "AssignedRightLabel";
            this.AssignedRightLabel.Size = new System.Drawing.Size(364, 50);
            this.AssignedRightLabel.TabIndex = 25;
            this.AssignedRightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AssignedLeftLabel
            // 
            this.AssignedLeftLabel.AutoEllipsis = true;
            this.AssignedLeftLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AssignedLeftLabel.Location = new System.Drawing.Point(204, 550);
            this.AssignedLeftLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AssignedLeftLabel.Name = "AssignedLeftLabel";
            this.AssignedLeftLabel.Size = new System.Drawing.Size(364, 50);
            this.AssignedLeftLabel.TabIndex = 24;
            this.AssignedLeftLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AssignLeftAction
            // 
            this.AssignLeftAction.Location = new System.Drawing.Point(576, 554);
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
            this.AssignRightAction.Location = new System.Drawing.Point(576, 504);
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
            this.AssignClickAction.Location = new System.Drawing.Point(576, 454);
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
            this.LeftLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LeftLabel.Location = new System.Drawing.Point(4, 550);
            this.LeftLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LeftLabel.Name = "LeftLabel";
            this.LeftLabel.Size = new System.Drawing.Size(192, 50);
            this.LeftLabel.TabIndex = 20;
            this.LeftLabel.Text = "Dial left action:";
            this.LeftLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RightLabel
            // 
            this.RightLabel.AutoSize = true;
            this.RightLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightLabel.Location = new System.Drawing.Point(4, 500);
            this.RightLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.RightLabel.Name = "RightLabel";
            this.RightLabel.Size = new System.Drawing.Size(192, 50);
            this.RightLabel.TabIndex = 19;
            this.RightLabel.Text = "Dial right action:";
            this.RightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ClickLabel
            // 
            this.ClickLabel.AutoSize = true;
            this.ClickLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClickLabel.Location = new System.Drawing.Point(4, 450);
            this.ClickLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ClickLabel.Name = "ClickLabel";
            this.ClickLabel.Size = new System.Drawing.Size(192, 50);
            this.ClickLabel.TabIndex = 18;
            this.ClickLabel.Text = "Dial clicked action:";
            this.ClickLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CommandsBox
            // 
            this.RootLayoutPanel.SetColumnSpan(this.CommandsBox, 3);
            this.CommandsBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommandsBox.Location = new System.Drawing.Point(3, 64);
            this.CommandsBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CommandsBox.Multiline = true;
            this.CommandsBox.Name = "CommandsBox";
            this.CommandsBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.CommandsBox.Size = new System.Drawing.Size(674, 382);
            this.CommandsBox.TabIndex = 17;
            this.CommandsBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CommandsBox_MouseClick);
            // 
            // SearcBoxLabel
            // 
            this.SearcBoxLabel.AutoSize = true;
            this.RootLayoutPanel.SetColumnSpan(this.SearcBoxLabel, 3);
            this.SearcBoxLabel.Location = new System.Drawing.Point(3, 0);
            this.SearcBoxLabel.Name = "SearcBoxLabel";
            this.SearcBoxLabel.Size = new System.Drawing.Size(332, 25);
            this.SearcBoxLabel.TabIndex = 16;
            this.SearcBoxLabel.Text = "Search for commands containing:";
            // 
            // SearchBox
            // 
            this.RootLayoutPanel.SetColumnSpan(this.SearchBox, 3);
            this.SearchBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchBox.Location = new System.Drawing.Point(3, 27);
            this.SearchBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SearchBox.Name = "SearchBox";
            this.SearchBox.Size = new System.Drawing.Size(674, 31);
            this.SearchBox.TabIndex = 15;
            this.SearchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // RootLayoutPanel
            // 
            this.RootLayoutPanel.ColumnCount = 3;
            this.RootLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.RootLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RootLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.RootLayoutPanel.Controls.Add(this.SearcBoxLabel, 0, 0);
            this.RootLayoutPanel.Controls.Add(this.SearchBox, 0, 1);
            this.RootLayoutPanel.Controls.Add(this.CommandsBox, 0, 2);
            this.RootLayoutPanel.Controls.Add(this.ClickLabel, 0, 3);
            this.RootLayoutPanel.Controls.Add(this.RightLabel, 0, 4);
            this.RootLayoutPanel.Controls.Add(this.LeftLabel, 0, 5);
            this.RootLayoutPanel.Controls.Add(this.AssignedClickLabel, 1, 3);
            this.RootLayoutPanel.Controls.Add(this.AssignedRightLabel, 1, 4);
            this.RootLayoutPanel.Controls.Add(this.AssignedLeftLabel, 1, 5);
            this.RootLayoutPanel.Controls.Add(this.AssignClickAction, 2, 3);
            this.RootLayoutPanel.Controls.Add(this.AssignRightAction, 2, 4);
            this.RootLayoutPanel.Controls.Add(this.AssignLeftAction, 2, 5);
            this.RootLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RootLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.RootLayoutPanel.Name = "RootLayoutPanel";
            this.RootLayoutPanel.RowCount = 8;
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.RootLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.RootLayoutPanel.Size = new System.Drawing.Size(680, 600);
            this.RootLayoutPanel.TabIndex = 27;
            // 
            // CustomOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RootLayoutPanel);
            this.Name = "CustomOptionsControl";
            this.Size = new System.Drawing.Size(680, 600);
            this.RootLayoutPanel.ResumeLayout(false);
            this.RootLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Label SearcBoxLabel;
        private System.Windows.Forms.TextBox SearchBox;
        private System.ComponentModel.BackgroundWorker SearchBackgroundWorker;
        private System.Windows.Forms.TableLayoutPanel RootLayoutPanel;
    }
}
