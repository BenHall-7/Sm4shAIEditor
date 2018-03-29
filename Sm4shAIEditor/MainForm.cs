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
        
        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Resources.Title;
            this.Icon = Properties.Resources.FoxLogo;
        }
        
        private void LoadFiles(string[] fileDirectories)
        {
            tree.AddFiles(fileDirectories, ref status_TB);
        }
        
        private void LoadFighters(string[] fighterDirectories)
        {
            tree.AddFighters(fighterDirectories, ref status_TB);
        }

        //maybe turn these into delegates instead of using if/else statements
        private void LoadATKD(string directory)
        {
            attack_data atkdFile = new attack_data(directory);

            TabPage atkdTab = new TabPage();
            atkdTab.Name = directory;
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
            fileTabContainer.SelectedTab = atkdTab;
        }

        private void LoadAIPD(string directory)
        {

        }

        private void LoadScript(string directory)
        {
            script scriptFile = new script(directory);

            TabPage entireScript = new TabPage();
            entireScript.Name = directory;
            string fileName = task_helper.GetFileName(directory);
            entireScript.Text = tree.aiFiles[directory] + "/" + fileName;

            TabControl actTabContainer = new TabControl();

            byte lastCmd = 0xff;
            foreach (script.Act act in scriptFile.acts.Keys)
            {
                TabPage actTab = new TabPage();
                actTab.Text = act.ID.ToString("X4");

                RichTextBox act_TB = new RichTextBox();

                //quick method to show script data, needs some organization in the future
                string text = "";
                int ifNestLevel = 0;
                foreach (script.Act.Cmd cmd in act.CmdList)
                {
                    //control the nested level spaces
                    string ifPadding = "";
                    if (cmd.ID == 8 || cmd.ID == 9)
                        ifNestLevel--;
                    for (int i = 0; i < ifNestLevel; i++)
                    {
                        ifPadding += "    ";
                    }
                    //account for the "else if" statement, which messes up the nest level
                    //This is because both those commands together should only change by 1 level, not 2
                    if (((cmd.ID == 6 || cmd.ID == 7) && lastCmd != 8) || cmd.ID == 8)
                        ifNestLevel++;

                    //define the params
                    string cmdParams = "";
                    for (int i = 0; i < cmd.ParamList.Count; i++)
                    {
                        UInt32 paramID = cmd.ParamList[i];
                        string paramString = "";
                        if (act.ScriptFloats.ContainsKey(cmd.ParamList[i]) && cmd.ID != 0x1b)
                            paramString = act.ScriptFloats[cmd.ParamList[i]].ToString();
                        else
                            paramString = "0x" + cmd.ParamList[i].ToString("X");

                        cmdParams += paramString;
                        if (i != cmd.ParamList.Count - 1)
                            cmdParams += ", ";
                    }

                    lastCmd = cmd.ID;

                    //whole string written out
                    text += ifPadding + script.CmdData[cmd.ID].Name + "(" + cmdParams + ")" + Environment.NewLine;
                }

                act_TB.Text = text;
                act_TB.Font = new Font(new FontFamily("Courier New"), 10f);
                act_TB.Parent = actTab;
                act_TB.Dock = DockStyle.Fill;
                actTabContainer.TabPages.Add(actTab);
            }
            actTabContainer.Parent = entireScript;
            actTabContainer.Dock = DockStyle.Fill;
            fileTabContainer.TabPages.Add(entireScript);
            fileTabContainer.SelectedTab = entireScript;
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
            string nodeDirectory = (string)treeView.SelectedNode.Tag;
            if (fileTabContainer.TabPages.ContainsKey(nodeDirectory))
            {
                fileTabContainer.SelectedIndex = fileTabContainer.TabPages.IndexOfKey(nodeDirectory);
            }
            else if (nodeDirectory != null)
            {
                //later on I might choose to make this into a dictionary
                string nodeFileName = task_helper.GetFileName(nodeDirectory);
                if (nodeFileName == task_helper.fileMagic.ElementAt(0).Key)
                    LoadATKD(nodeDirectory);
                else if (nodeFileName == task_helper.fileMagic.ElementAt(1).Key || nodeFileName == task_helper.fileMagic.ElementAt(2).Key)
                    LoadAIPD(nodeDirectory);
                else if (nodeFileName == task_helper.fileMagic.ElementAt(3).Key)
                    LoadScript(nodeDirectory);
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
