namespace GitLooker.Controls
{
    partial class RepoList
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepoList));
            this.repoText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // repoText
            // 
            this.repoText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.repoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.repoText.Location = new System.Drawing.Point(0, 0);
            this.repoText.Multiline = true;
            this.repoText.Name = "repoText";
            this.repoText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.repoText.Size = new System.Drawing.Size(658, 388);
            this.repoText.TabIndex = 0;
            // 
            // RepoList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(658, 388);
            this.Controls.Add(this.repoText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RepoList";
            this.Text = "Repo list";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox repoText;
    }
}