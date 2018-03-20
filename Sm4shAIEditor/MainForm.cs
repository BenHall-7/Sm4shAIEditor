﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Sm4shAIEditor.FileTypes;

namespace Sm4shAIEditor
{
    public partial class MainForm : Form
    {
        public static AITree tree = new AITree();
        public List<TabPage> fileTabs = new List<TabPage>();//for keeping files open - currently unused
        public TabPage previewTab = new TabPage();//for looking at a file temporarily - currently unused
        
        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Resources.Title;
            this.Icon = Properties.Resources.FoxLogo;
        }

        //sub method used for loading adding files with no associated fighter
        private void LoadFiles(string[] fileDirectories)
        {
            tree.AddFiles(fileDirectories, ref status_TB);
        }

        //sub method for loading fighters and their associated files
        private void LoadFighters(string[] fighterDirectories)
        {
            tree.AddFighters(fighterDirectories, ref status_TB);
        }

        //maybe a method to load the tab data would be better suited for the class itself
        private void LoadATKD(string directory)
        {
            attack_data atkdFile = new attack_data(directory);

            TabPage atkdTab = new TabPage();
            atkdTab.Tag = directory;
            string fileName = task_helper.GetFileName(directory);
            atkdTab.Text = tree.aiFiles[directory]+"/"+fileName;

            DataGridView atkdTabData = new DataGridView();
            atkdTabData.RowCount = (int)atkdFile.EntryCount;
            atkdTabData.ColumnCount = 7;
            atkdTabData.Columns[0].HeaderText = "Subaction Index";
            atkdTabData.Columns[1].HeaderText = "First Frame";
            atkdTabData.Columns[2].HeaderText = "Last Frame";
            atkdTabData.Columns[3].HeaderText = "X1";
            atkdTabData.Columns[4].HeaderText = "X2";
            atkdTabData.Columns[5].HeaderText = "Y1";
            atkdTabData.Columns[6].HeaderText = "Y2";
            foreach (DataGridViewRow row in atkdTabData.Rows)
            {
                row.Cells[0].Value = atkdFile.attacks[row.Index].SubactionID;
                row.Cells[1].Value = atkdFile.attacks[row.Index].FirstFrame;
                row.Cells[2].Value = atkdFile.attacks[row.Index].LastFrame;
                row.Cells[3].Value = atkdFile.attacks[row.Index].X1;
                row.Cells[4].Value = atkdFile.attacks[row.Index].X2;
                row.Cells[5].Value = atkdFile.attacks[row.Index].Y1;
                row.Cells[6].Value = atkdFile.attacks[row.Index].Y2;
            }
            atkdTabData.Parent = atkdTab;
            atkdTabData.Dock = DockStyle.Fill;
            atkdTabData.ReadOnly = true;
            atkdTabData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            atkdTabData.AutoResizeColumns();
            fileTabContainer.TabPages.Add(atkdTab);
        }

        private void LoadAIPD(string directory)
        {

        }

        private void LoadScript(string directory)
        {
            script scriptFile = new script(directory);

            TabPage scriptTab = new TabPage();
            scriptTab.Tag = directory;
            string fileName = task_helper.GetFileName(directory);
            scriptTab.Text = tree.aiFiles[directory] + "/" + fileName;
            RichTextBox scriptTabData = new RichTextBox();
            string text = "";
            foreach (script.Routine routine in scriptFile.Routines.Keys)
            {
                //Quick and Dirty and method to show script data. NOT PERMANENT
                text += routine.RoutineID.ToString("X") + "{" + Environment.NewLine;
                foreach (script.Routine.Command cmd in routine.CommandList)
                {
                    string cmdParams = "";
                    for (int i = 0; i < cmd.ParamList.Count; i++)
                    {
                        cmdParams += cmd.ParamList[i].ToString("X");
                        if (i != cmd.ParamList.Count - 1)
                            cmdParams += ", ";
                    }
                    text += "    " + script.CmdNames[cmd.CmdID] + "(" + cmdParams + ")" + Environment.NewLine;
                }
                text += "}" + Environment.NewLine;
            }
            scriptTabData.Text = text;
            scriptTabData.Font = new Font(new FontFamily("Courier New"),10f);
            scriptTabData.Parent = scriptTab;
            scriptTabData.Dock = DockStyle.Fill;
            fileTabContainer.TabPages.Add(scriptTab);
        }

        private void UpdateTreeView()
        {
            treeView.Nodes.Clear();
            string nodeName;
            string nodeTag;

            string[] fileDirs = tree.aiFiles.Keys.ToArray();
            string[] fighters = tree.fighters.ToArray();
            foreach (string fighter in fighters)
            {
                nodeName = fighter;
                treeView.Nodes.Add(nodeName, nodeName);//given key is the fighter name, which allows the search method below
            }

            foreach (string key in fileDirs)
            {
                string owner = tree.aiFiles[key];
                nodeTag = key;
                if (owner == null)
                {
                    nodeName = key;
                    TreeNode node = new TreeNode(nodeName);
                    node.Tag = nodeTag;
                    treeView.Nodes.Add(node);
                }
                else
                {
                    TreeNode fighterNode = treeView.Nodes.Find(owner, false)[0];//returns array, but should only have 1 element
                    nodeName = task_helper.GetFileName(key);
                    TreeNode child = new TreeNode(nodeName);
                    child.Tag = nodeTag;
                    fighterNode.Nodes.Add(child);
                }
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Properties.Resources.OpenFileFilter;
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFiles(openFile.FileNames);
            }

            UpdateTreeView();
        }

        private void openFighterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderSelectDialog openFighter = new FolderSelectDialog(true);
            if (openFighter.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFighters(openFighter.SelectedPaths);
            }

            UpdateTreeView();
        }

        private void openAllFightersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderSelectDialog openAllFighters = new FolderSelectDialog(false);
            if (openAllFighters.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] fighterDirectories = Directory.EnumerateDirectories(openAllFighters.SelectedPath).ToArray();
                LoadFighters(fighterDirectories);
            }

            UpdateTreeView();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //TODO: unload any files previously selected here

            //only the main files have a tag attribute; it stores the file directory and uses it as a unique identifier
            string nodeTag = (string)treeView.SelectedNode.Tag;
            if (nodeTag != null)
            {
                //later on I might choose to make this into a dictionary
                string nodeFileName = task_helper.GetFileName(nodeTag);
                if (nodeFileName == task_helper.fileMagic.ElementAt(0).Key)
                    LoadATKD(nodeTag);
                else if (nodeFileName == task_helper.fileMagic.ElementAt(1).Key || nodeFileName == task_helper.fileMagic.ElementAt(2).Key)
                    LoadAIPD(nodeTag);
                else if (nodeFileName == task_helper.fileMagic.ElementAt(3).Key)
                    LoadScript(nodeTag);
            }
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void fileTabContainer_ControlAdded(object sender, ControlEventArgs e)
        {
            if (fileTabContainer.TabCount != 0)
                fileTabContainer.Visible = true;
        }

        private void fileTabContainer_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (fileTabContainer.TabCount == 0)
                fileTabContainer.Visible = false;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
