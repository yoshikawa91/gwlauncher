﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GW_Launcher
{
    public partial class ModManager : Form
    {
        private readonly Account account;
        public int Selected { get; set; }

        public ModManager(Account account)
        {
            this.account = account;

            InitializeComponent();

            Text = @"Mod Manager for " + this.account.character;
        }

        private void RefreshUI()
        {
            listViewAvailableMods.Items.Clear();
            foreach (var mod in account.mods)
            {
                var name = System.IO.Path.GetFileName(mod.fileName);
                var path = System.IO.Path.GetDirectoryName(mod.fileName);
                var item = new ListViewItem(new string[] {
                        name,
                        path
                    }, mod.fileName)
                {
                    Checked = mod.active
                };

                switch (mod.type)
                {
                    case ModType.kModTypeTexmod:
                        listViewAvailableMods.Groups[0].Items.Add(item);
                        break;

                    case ModType.kModTypeDLL:
                        listViewAvailableMods.Groups[1].Items.Add(item);
                        break;
                }
                
                listViewAvailableMods.Items.Add(item);
            }
        }

        private void TexmodManager_Load(object sender, EventArgs e)
        {
            if(account.mods == null)
            {
                return;
            }

            RefreshUI();
        }

        private void listViewAvailableMods_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var mod = account.mods[e.Item.Index];
            mod.active = e.Item.Checked;
            Program.accounts.Save();
        }

        private void addModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = @"Select Mod File to Use",
                Filter = @"Mod Files (*.dll;*.zip;*.tpf)|*.dll;*.zip;*.tpf|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var mod = new Mod
                {
                    fileName = openFileDialog.FileName,
                    active = false
                };
                switch (openFileDialog.FileName.Split('.').Last())
                {
                    case "dll":
                        mod.type = ModType.kModTypeDLL;
                        break;
                    case "zip":
                    case "tpf":
                        mod.type = ModType.kModTypeTexmod;
                        break;
                }
                if (account.mods == null)
                    account.mods = new List<Mod>();
                account.mods.Add(mod);
                Program.accounts.Save();
                RefreshUI();
            }
        }

        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.mutex.WaitOne();
            var selectedthing = listViewAvailableMods.SelectedIndices[0];
            var selectedmod = account.mods[selectedthing];
            account.mods.Remove(selectedmod);
            Program.accounts.Save();
            RefreshUI();
            Program.mutex.ReleaseMutex();
        }

        private void listViewAvailableMods_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}