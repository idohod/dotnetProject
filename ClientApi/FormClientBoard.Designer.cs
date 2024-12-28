namespace Client
{
    partial class FormClientBoard
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelBoard;
        private System.Windows.Forms.ComboBox comboBoxTime;
        private System.Windows.Forms.Label lblTimeLeft;
        private System.Windows.Forms.Button btnClearDrawings;
        private System.Windows.Forms.Timer turnTimer;
        private System.Windows.Forms.Timer animationTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelBoard = new System.Windows.Forms.Panel();
            this.comboBoxTime = new System.Windows.Forms.ComboBox();
            this.lblTimeLeft = new System.Windows.Forms.Label();
            this.btnClearDrawings = new System.Windows.Forms.Button();
            this.turnTimer = new System.Windows.Forms.Timer(this.components);
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // panelBoard
            // 
            this.panelBoard.Location = new System.Drawing.Point(22, 22);
            this.panelBoard.Margin = new System.Windows.Forms.Padding(6);
            this.panelBoard.Name = "panelBoard";
            this.panelBoard.Size = new System.Drawing.Size(440, 930);
            this.panelBoard.TabIndex = 0;
            this.panelBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBoard_Paint);
            this.panelBoard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBoard_MouseDown);
            this.panelBoard.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelBoard_MouseMove);
            this.panelBoard.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelBoard_MouseUp);
            // 
            // comboBoxTime
            // 
            this.comboBoxTime.FormattingEnabled = true;
            this.comboBoxTime.Location = new System.Drawing.Point(495, 22);
            this.comboBoxTime.Margin = new System.Windows.Forms.Padding(6);
            this.comboBoxTime.Name = "comboBoxTime";
            this.comboBoxTime.Size = new System.Drawing.Size(180, 32);
            this.comboBoxTime.TabIndex = 1;
            this.comboBoxTime.SelectedIndexChanged += new System.EventHandler(this.comboBoxTime_SelectedIndexChanged);
            // 
            // lblTimeLeft
            // 
            this.lblTimeLeft.AutoSize = true;
            this.lblTimeLeft.Location = new System.Drawing.Point(495, 92);
            this.lblTimeLeft.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblTimeLeft.Name = "lblTimeLeft";
            this.lblTimeLeft.Size = new System.Drawing.Size(92, 25);
            this.lblTimeLeft.TabIndex = 2;
            this.lblTimeLeft.Text = "Time left:";
            // 
            // btnClearDrawings
            // 
            this.btnClearDrawings.Location = new System.Drawing.Point(495, 148);
            this.btnClearDrawings.Margin = new System.Windows.Forms.Padding(6);
            this.btnClearDrawings.Name = "btnClearDrawings";
            this.btnClearDrawings.Size = new System.Drawing.Size(183, 42);
            this.btnClearDrawings.TabIndex = 3;
            this.btnClearDrawings.Text = "Clear Drawings";
            this.btnClearDrawings.UseVisualStyleBackColor = true;
            this.btnClearDrawings.Click += new System.EventHandler(this.btnClearDrawings_Click);
            // 
            // turnTimer
            // 
            this.turnTimer.Interval = 1000;
            this.turnTimer.Tick += new System.EventHandler(this.TurnTimer_Tick);
            // 
            // animationTimer
            // 
            this.animationTimer.Tick += new System.EventHandler(this.animationTimer_Tick);
            // 
            // FormClientBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 1060);
            this.Controls.Add(this.btnClearDrawings);
            this.Controls.Add(this.lblTimeLeft);
            this.Controls.Add(this.comboBoxTime);
            this.Controls.Add(this.panelBoard);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FormClientBoard";
            this.Text = "Half Chess Client Board";
            this.Load += new System.EventHandler(this.FormClientBoard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
