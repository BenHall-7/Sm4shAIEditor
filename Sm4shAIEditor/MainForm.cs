﻿using Sm4shAIEditor.Static;
using System;
using System.IO;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    public partial class MainForm : Form
    {
        public static AITree tree = new AITree();
        public static bool projectActive { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            Text = Properties.Resources.Title;
            Icon = Properties.Resources.FoxLogo;
            SetProjectStatus(false);
        }

        public void SetProjectStatus(bool active)
        {
            if (active) Console.WriteLine("Project loaded: '{0}'", util.workDir);
            else if (projectActive == true) Console.WriteLine("Project unloaded: '{0}'", util.workDir);
            projectActive = active;
            newProjectToolStripMenuItem.Enabled = !active;
            openProjectToolStripMenuItem.Enabled = !active;
            addGameFilesToolStripMenuItem.Enabled = active;
            addCompiledFilesToolStripMenuItem.Enabled = active;
            compileToolStripMenuItem.Enabled = active;
            closeProjectToolStripMenuItem.Enabled = active;
        }

        private void UpdateTreeView()
        {
            treeView.Nodes.Clear();
            foreach (AITree.AIFt ft in tree.fighters)
            {
                TreeNode ftNode = new TreeNode(ft.name);
                foreach (AITree.AIFt.AIFile file in ft.files)
                    ftNode.Nodes.Add(AITree.AITypeToString[file.type]);
                treeView.Nodes.Add(ftNode);
            }
        }
        
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            FighterSelection selector = new FighterSelection(AITree.AISource.game_file);
            if (selector.ShowDialog() != DialogResult.OK)
                return;
            tree.InitNewProject(selector.selFighters, selector.selTypes);
            UpdateTreeView();
            SetProjectStatus(true);
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            tree.InitOpenProject();
            UpdateTreeView();
            SetProjectStatus(true);
        }

        private void addGameFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            FighterSelection selector = new FighterSelection(AITree.AISource.game_file);
            if (selector.ShowDialog() != DialogResult.OK)
                return;
            tree.AddProjectFiles(selector.selFighters, selector.selTypes, AITree.AISource.game_file);
            UpdateTreeView();
        }

        private void addCompiledFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            FighterSelection selector = new FighterSelection(AITree.AISource.compiled);
            if (selector.ShowDialog() != DialogResult.OK)
                return;
            tree.AddProjectFiles(selector.selFighters, selector.selTypes, AITree.AISource.compiled);
            UpdateTreeView();
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tree.fighters.Clear();
            UpdateTreeView();
            SetProjectStatus(false);
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            FighterSelection selector = new FighterSelection(AITree.AISource.work);
            if (selector.ShowDialog() != DialogResult.OK)
                return;
            foreach (var ft in selector.selFighters)
            {
                foreach (var type in selector.selTypes)
                {
                    string pathIn = AITree.GetFolderPath(ft, type, AITree.AISource.work);
                    if (Directory.Exists(pathIn))
                    {
                        string pathOut = AITree.GetFolderPath(ft, type, AITree.AISource.compiled);
                        aism.AssembleFolder(pathIn, pathOut);
                    }
                }
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
        }

        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = treeView.SelectedNode;
            if (node == null) return;
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    AITree.AIType type = AITree.StringToAIType[child.Text];
                    string pathIn = AITree.GetFolderPath(node.Text, type, AITree.AISource.work);
                    string pathOut = AITree.GetFolderPath(node.Text, type, AITree.AISource.compiled);
                    aism.AssembleFolder(pathIn, pathOut);
                }
            }
            else if (node.Parent != null)
            {
                AITree.AIType type = AITree.StringToAIType[node.Text];
                string pathIn = AITree.GetFolderPath(node.Parent.Text, type, AITree.AISource.work);
                string pathOut = AITree.GetFolderPath(node.Parent.Text, type, AITree.AISource.compiled);
                aism.AssembleFolder(pathIn, pathOut);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config config = new Config(projectActive);
            config.ShowDialog();
        }

        private bool CheckConfig()
        {
            Config config = new Config();
            if (!config.check && (config.ShowDialog() != DialogResult.OK))
                return false;
            return true;
        }
    }
}
