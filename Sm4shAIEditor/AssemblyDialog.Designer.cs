﻿namespace Sm4shAIEditor
{
    partial class AssemblyDialog
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
            this.rB_assemble = new System.Windows.Forms.RadioButton();
            this.rB_disassemble = new System.Windows.Forms.RadioButton();
            this.gB_filetypes = new System.Windows.Forms.GroupBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.gB_assemblyScope = new System.Windows.Forms.GroupBox();
            this.gB_disassemblyScope = new System.Windows.Forms.GroupBox();
            this.rB_asmFromTabs = new System.Windows.Forms.RadioButton();
            this.rB_asmFromFolder = new System.Windows.Forms.RadioButton();
            this.rB_disasmFromTabs = new System.Windows.Forms.RadioButton();
            this.rB_disasmFromTree = new System.Windows.Forms.RadioButton();
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_accept = new System.Windows.Forms.Button();
            this.cB_disasmTreeAndTabs = new System.Windows.Forms.CheckBox();
            this.gB_filetypes.SuspendLayout();
            this.gB_assemblyScope.SuspendLayout();
            this.gB_disassemblyScope.SuspendLayout();
            this.SuspendLayout();
            // 
            // rB_assemble
            // 
            this.rB_assemble.AutoSize = true;
            this.rB_assemble.Location = new System.Drawing.Point(12, 12);
            this.rB_assemble.Name = "rB_assemble";
            this.rB_assemble.Size = new System.Drawing.Size(70, 17);
            this.rB_assemble.TabIndex = 0;
            this.rB_assemble.TabStop = true;
            this.rB_assemble.Text = "Assemble";
            this.rB_assemble.UseVisualStyleBackColor = true;
            this.rB_assemble.CheckedChanged += new System.EventHandler(this.rB_assemble_CheckedChanged);
            // 
            // rB_disassemble
            // 
            this.rB_disassemble.AutoSize = true;
            this.rB_disassemble.Location = new System.Drawing.Point(88, 12);
            this.rB_disassemble.Name = "rB_disassemble";
            this.rB_disassemble.Size = new System.Drawing.Size(84, 17);
            this.rB_disassemble.TabIndex = 1;
            this.rB_disassemble.TabStop = true;
            this.rB_disassemble.Text = "Disassemble";
            this.rB_disassemble.UseVisualStyleBackColor = true;
            // 
            // gB_filetypes
            // 
            this.gB_filetypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gB_filetypes.Controls.Add(this.checkBox3);
            this.gB_filetypes.Controls.Add(this.checkBox2);
            this.gB_filetypes.Controls.Add(this.checkBox1);
            this.gB_filetypes.Location = new System.Drawing.Point(12, 35);
            this.gB_filetypes.Name = "gB_filetypes";
            this.gB_filetypes.Size = new System.Drawing.Size(260, 41);
            this.gB_filetypes.TabIndex = 2;
            this.gB_filetypes.TabStop = false;
            this.gB_filetypes.Text = "File Types";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(125, 20);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(58, 17);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "Scripts";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(68, 20);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(51, 17);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "AIPD";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(7, 20);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(55, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "ATKD";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // gB_assemblyScope
            // 
            this.gB_assemblyScope.Controls.Add(this.rB_asmFromFolder);
            this.gB_assemblyScope.Controls.Add(this.rB_asmFromTabs);
            this.gB_assemblyScope.Location = new System.Drawing.Point(12, 83);
            this.gB_assemblyScope.Name = "gB_assemblyScope";
            this.gB_assemblyScope.Size = new System.Drawing.Size(260, 65);
            this.gB_assemblyScope.TabIndex = 3;
            this.gB_assemblyScope.TabStop = false;
            this.gB_assemblyScope.Text = "Assembly Scope";
            // 
            // gB_disassemblyScope
            // 
            this.gB_disassemblyScope.Controls.Add(this.cB_disasmTreeAndTabs);
            this.gB_disassemblyScope.Controls.Add(this.rB_disasmFromTree);
            this.gB_disassemblyScope.Controls.Add(this.rB_disasmFromTabs);
            this.gB_disassemblyScope.Location = new System.Drawing.Point(12, 154);
            this.gB_disassemblyScope.Name = "gB_disassemblyScope";
            this.gB_disassemblyScope.Size = new System.Drawing.Size(260, 86);
            this.gB_disassemblyScope.TabIndex = 4;
            this.gB_disassemblyScope.TabStop = false;
            this.gB_disassemblyScope.Text = "Disassembly Scope";
            // 
            // rB_asmFromTabs
            // 
            this.rB_asmFromTabs.AutoSize = true;
            this.rB_asmFromTabs.Location = new System.Drawing.Point(6, 19);
            this.rB_asmFromTabs.Name = "rB_asmFromTabs";
            this.rB_asmFromTabs.Size = new System.Drawing.Size(120, 17);
            this.rB_asmFromTabs.TabIndex = 0;
            this.rB_asmFromTabs.TabStop = true;
            this.rB_asmFromTabs.Text = "Assemble from Tabs";
            this.rB_asmFromTabs.UseVisualStyleBackColor = true;
            this.rB_asmFromTabs.CheckedChanged += new System.EventHandler(this.rB_asmFromTabs_CheckedChanged);
            // 
            // rB_asmFromFolder
            // 
            this.rB_asmFromFolder.AutoSize = true;
            this.rB_asmFromFolder.Location = new System.Drawing.Point(6, 42);
            this.rB_asmFromFolder.Name = "rB_asmFromFolder";
            this.rB_asmFromFolder.Size = new System.Drawing.Size(125, 17);
            this.rB_asmFromFolder.TabIndex = 1;
            this.rB_asmFromFolder.TabStop = true;
            this.rB_asmFromFolder.Text = "Assemble from Folder";
            this.rB_asmFromFolder.UseVisualStyleBackColor = true;
            // 
            // rB_disasmFromTabs
            // 
            this.rB_disasmFromTabs.AutoSize = true;
            this.rB_disasmFromTabs.Location = new System.Drawing.Point(6, 19);
            this.rB_disasmFromTabs.Name = "rB_disasmFromTabs";
            this.rB_disasmFromTabs.Size = new System.Drawing.Size(134, 17);
            this.rB_disasmFromTabs.TabIndex = 0;
            this.rB_disasmFromTabs.TabStop = true;
            this.rB_disasmFromTabs.Text = "Disassemble from Tabs";
            this.rB_disasmFromTabs.UseVisualStyleBackColor = true;
            this.rB_disasmFromTabs.CheckedChanged += new System.EventHandler(this.rB_disasmFromTabs_CheckedChanged);
            // 
            // rB_disasmFromTree
            // 
            this.rB_disasmFromTree.AutoSize = true;
            this.rB_disasmFromTree.Location = new System.Drawing.Point(6, 42);
            this.rB_disasmFromTree.Name = "rB_disasmFromTree";
            this.rB_disasmFromTree.Size = new System.Drawing.Size(132, 17);
            this.rB_disasmFromTree.TabIndex = 1;
            this.rB_disasmFromTree.TabStop = true;
            this.rB_disasmFromTree.Text = "Disassemble from Tree";
            this.rB_disasmFromTree.UseVisualStyleBackColor = true;
            // 
            // button_cancel
            // 
            this.button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_cancel.Location = new System.Drawing.Point(197, 251);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 5;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_accept
            // 
            this.button_accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_accept.Location = new System.Drawing.Point(107, 251);
            this.button_accept.Name = "button_accept";
            this.button_accept.Size = new System.Drawing.Size(75, 23);
            this.button_accept.TabIndex = 6;
            this.button_accept.Text = "Accept";
            this.button_accept.UseVisualStyleBackColor = true;
            this.button_accept.Click += new System.EventHandler(this.button_accept_Click);
            // 
            // cB_disasmTreeAndTabs
            // 
            this.cB_disasmTreeAndTabs.AutoSize = true;
            this.cB_disasmTreeAndTabs.Location = new System.Drawing.Point(27, 63);
            this.cB_disasmTreeAndTabs.Name = "cB_disasmTreeAndTabs";
            this.cB_disasmTreeAndTabs.Size = new System.Drawing.Size(133, 17);
            this.cB_disasmTreeAndTabs.TabIndex = 2;
            this.cB_disasmTreeAndTabs.Text = "Include changed Tabs";
            this.cB_disasmTreeAndTabs.UseVisualStyleBackColor = true;
            this.cB_disasmTreeAndTabs.CheckedChanged += new System.EventHandler(this.cB_disasmTreeAndTabs_CheckedChanged);
            // 
            // AssemblyDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 286);
            this.Controls.Add(this.button_accept);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.gB_disassemblyScope);
            this.Controls.Add(this.gB_assemblyScope);
            this.Controls.Add(this.gB_filetypes);
            this.Controls.Add(this.rB_disassemble);
            this.Controls.Add(this.rB_assemble);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AssemblyDialog";
            this.Text = "Assembly Options";
            this.Load += new System.EventHandler(this.AssemblyDialog_Load);
            this.gB_filetypes.ResumeLayout(false);
            this.gB_filetypes.PerformLayout();
            this.gB_assemblyScope.ResumeLayout(false);
            this.gB_assemblyScope.PerformLayout();
            this.gB_disassemblyScope.ResumeLayout(false);
            this.gB_disassemblyScope.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rB_assemble;
        private System.Windows.Forms.RadioButton rB_disassemble;
        private System.Windows.Forms.GroupBox gB_filetypes;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox gB_assemblyScope;
        private System.Windows.Forms.GroupBox gB_disassemblyScope;
        private System.Windows.Forms.RadioButton rB_asmFromFolder;
        private System.Windows.Forms.RadioButton rB_asmFromTabs;
        private System.Windows.Forms.RadioButton rB_disasmFromTree;
        private System.Windows.Forms.RadioButton rB_disasmFromTabs;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_accept;
        private System.Windows.Forms.CheckBox cB_disasmTreeAndTabs;
    }
}