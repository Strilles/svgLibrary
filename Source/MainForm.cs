using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace svgLibrary
{
    public partial class MainForm : Form
    {
        bool docLoaded = false;

        static int foldersFound = 0;
        static int filesFound = 0;

        String selectedDocument = "";
        String selectedPath = "";

        public MainForm()
        {
            InitializeComponent();

            if (Properties.Settings.Default.firstUse)
            {
                folderBrowserDialog.ShowDialog();

                while(folderBrowserDialog.SelectedPath == "")
                {
                    return;
                }
                Properties.Settings.Default["defaultRootPath"] = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default["firstUse"] = false;
                Properties.Settings.Default.Save();

            }
            
            PopulateTreeView();
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;

        }

        private void PopulateTreeView()
        {
            TreeNode rootNode;
            String rootPath = Properties.Settings.Default.defaultRootPath;
            DirectoryInfo info = new DirectoryInfo(rootPath);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                folderTree.Nodes.Add(rootNode);
            }
        }

        private void ResetTreeView()
        {
            folderTree.Nodes.Clear();
            PopulateTreeView();
        }

        private void GetDirectories(DirectoryInfo[] subDirs,
            TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            documentListView.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            selectedPath = nodeDirInfo.FullName;
            
            
            foreach (FileInfo file in nodeDirInfo.EnumerateFiles()
                .Where(s => s.ToString().EndsWith(".png") || s.ToString().EndsWith(".svg")))
            {
                item = new ListViewItem(file.Name, 1);
                item.ToolTipText = file.DirectoryName.ToString();
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"),
             new ListViewItem.ListViewSubItem(item,
                file.LastAccessTime.ToShortDateString())};

                item.SubItems.AddRange(subItems);
                documentListView.Items.Add(item);
            }

            documentListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private bool RecursiveSearch(IEnumerable parent, String search)
        {
            StringComparison stringCompare = StringComparison.OrdinalIgnoreCase;
            foreach (TreeNode node in parent)
            {
                if (node.Text.IndexOf(search, stringCompare) >= 0)
                {
                    foldersFound++;
                    node.BackColor = Color.Yellow;
                    folderTree.SelectedNode = node;
                    node.Expand();
                } else
                {
                    node.BackColor = Color.White;
                    node.Collapse();
                }
                if (RecursiveSearch(node.Nodes, search))
                    return true;
            }
            return false;
        }

        private void ToolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Return && toolStripSearchBox.Text != "")
            {
                //Reset Search Results
                filesFound = 0;
                foldersFound = 0;
                // Search Folders
                TreeNodeCollection nodes = folderTree.Nodes;
                RecursiveSearch(nodes, toolStripSearchBox.Text);
                

                // Search Files
                TreeNode newSelected = folderTree.SelectedNode;

                documentListView.Items.Clear();
                DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
                
                ListViewItem.ListViewSubItem[] subItems;
                ListViewItem item = null;

                foreach (FileInfo file in nodeDirInfo.EnumerateFiles("*", SearchOption.AllDirectories)
                    .Where(s => s.ToString().EndsWith(".png") || s.ToString().EndsWith(".svg"))
                    .Where(s => s.ToString().IndexOf(toolStripSearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    item = new ListViewItem(file.Name, 1);
                    item.ToolTipText = file.DirectoryName.ToString();
                    subItems = new ListViewItem.ListViewSubItem[]
                        { new ListViewItem.ListViewSubItem(item, "File"),
             new ListViewItem.ListViewSubItem(item,
                file.LastAccessTime.ToShortDateString())};

                    item.SubItems.AddRange(subItems);
                    documentListView.Items.Add(item);
                    filesFound++;
                }

                documentListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                if (documentListView.Items.Count == 0 && foldersFound == 0)
                {
                    MessageBox.Show("No Items found!");
                } else
                {
                    MessageBox.Show($"Found {filesFound} files & {foldersFound} folders matching search!");
                }
                    

            } else if (e.KeyChar == (Char)Keys.Return && toolStripSearchBox.Text == "")
            {
                ResetTreeView();
                documentListView.Items.Clear();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String fullPath = "";
            if (documentListView.SelectedItems.Count > 0)
                fullPath = documentListView.SelectedItems[0].ToolTipText + @"\" + documentListView.SelectedItems[0].Text.ToString();

            String uri = @"file:///" + fullPath;
            if (uri != @"file:///")
                loadDoc(uri);

            selectedDocument = fullPath;
        }

        void loadDoc(String path)
        {
            webBrowser.Url = new Uri(path);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            docLoaded = true;
            if (webBrowser.Url.ToString().EndsWith(@".png"))
            {
                resizePNG();
            } else if (webBrowser.Url.ToString().EndsWith(@".svg"))
            {
                if (CheckSVGVersion() == "1.0")
                    resizeSVG();
            }

        }

        private string CheckSVGVersion()
        {
            String contentStr = "";
            HtmlElementCollection elements = webBrowser.Document.GetElementsByTagName("svg");
            foreach (HtmlElement element in elements)
            {
                contentStr = element.GetAttribute("version");
                if (contentStr == "1.0")
                {
                    return contentStr;
                }
            }
            return contentStr;
        }

        void resizeSVG()
        {
            string doc = File.ReadAllText(selectedDocument);
            Regex regex = new Regex("width=(.*?)x");
            doc = regex.Replace(doc, "width=" + (char)34 + webBrowser.Size.Width.ToString() + "px");
            
            regex = new Regex("height=(.*?)x");
            doc = regex.Replace(doc, "height=" + (char)34 + webBrowser.Size.Height.ToString() + "px");
            File.WriteAllText(selectedDocument, doc);
            webBrowser.Refresh();
        }

        void resizePNG()
        {
            double origSizeWidth, origSizeHeight;
            origSizeWidth = webBrowser.Size.Width; //get the original size of browser 
            origSizeHeight = webBrowser.Size.Height;

            var img = webBrowser.Document.GetElementsByTagName("img")
                .Cast<HtmlElement>().FirstOrDefault();
            double origPicWidth, origPicHeight;
            origPicWidth = double.Parse(img.GetAttribute("WIDTH")); //save the image width
            origPicHeight = double.Parse(img.GetAttribute("HEIGHT"));//and height

            double tempW, tempY, widthScale, heightScale;
            double scale = 0;
            
            if (origPicWidth > origSizeWidth)
            {
                widthScale = origSizeWidth / origPicWidth; //find out the scale factor for the width
                heightScale = origSizeHeight / origPicHeight; //scale factor for height

                scale = Math.Min(widthScale, heightScale);//determine which scale to use from the smallest

                tempW = origPicWidth * scale; //multiply picture original width by the scale
                tempY = origPicHeight * scale; // multiply original picture height by the scale
                img.SetAttribute("WIDTH", tempW.ToString()); // set your attributes
                img.SetAttribute("HEIGHT", tempY.ToString());
            } else
            {
                img.SetAttribute("WIDTH", origSizeWidth.ToString());
                img.SetAttribute("HEIGHT", origSizeHeight.ToString());
            }
        }

        private void webBrowser1_Resize(object sender, EventArgs e)
        {
            while (!docLoaded)
            {
                Console.WriteLine("Document Not Loaded");
                return;
            }
            if (webBrowser.Url.ToString().Contains(@".png"))
            {
                resizePNG();
            }
            if (webBrowser.Url.ToString().Contains(@".svg"))
            {
                if (CheckSVGVersion() == "1.0")
                    resizeSVG();
            }
        }

        private void toolStripTextBox1_Leave(object sender, EventArgs e)
        {
            if (toolStripSearchBox.Text == "")
            {
                toolStripSearchBox.Text = "Search...";
                toolStripSearchBox.ForeColor = SystemColors.InactiveCaption;
            }

        }

        private void toolStripTextBox1_Enter(object sender, EventArgs e)
        {
            toolStripSearchBox.Text = "";
            toolStripSearchBox.ForeColor = SystemColors.WindowText;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
           _ = folderBrowserDialog.ShowDialog();
            
            Properties.Settings.Default["defaultRootPath"] = folderBrowserDialog.SelectedPath;
            Properties.Settings.Default.Save();
            ResetTreeView();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (documentListView.SelectedItems.Count >= 1)
                Process.Start(documentListView.SelectedItems[0].ToolTipText);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                rightClickMenuStrip.Show(Cursor.Position);
            }

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this file?", "Delete File?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.OK)
            {
                if (File.Exists(selectedDocument))
                    File.Delete(selectedDocument);
                documentListView.SelectedItems[0].Remove();

            } else
            {
                return;
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentListView.LabelEdit = true;
            documentListView.SelectedItems[0].BeginEdit();
            
            
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (File.Exists(selectedDocument))
                File.Move(selectedDocument, selectedPath + @"\" + e.Label);
            selectedDocument = selectedPath + @"\" + e.Label;
            documentListView.LabelEdit = false;
        }

        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanForm scanForm = new ScanForm();
            scanForm.Show();
        }
    }
}
