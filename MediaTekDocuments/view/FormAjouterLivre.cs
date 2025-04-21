using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using Ubiety.Dns.Core.Records;

namespace MediaTekDocuments.view
{
    public partial class FormAjouterLivre : Form
    {
        private readonly FrmMediatekController controller;
        public Livre NouveauLivre { get; private set; }
        
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
     


        public FormAjouterLivre()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            RemplirComboBox();
            this.button2.Click += new System.EventHandler(this.button2_Click);
        }

        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbb)
        {
            bdg.DataSource = lesCategories;
            cbb.DataSource = bdg;
            if (cbb.Items.Count > 0)
            {
                cbb.SelectedIndex = -1;
            }
        }

        private void RemplirComboBox() 
        {
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbbGenre);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbbPublic);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbbRayon);
        }


        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void cbbGenre_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnValider(object sender, EventArgs e)
        {
            if (cbbGenre.SelectedIndex == -1 || cbbPublic.SelectedIndex == -1 || cbbRayon.SelectedIndex == -1)
            {
                MessageBox.Show("Tous les champs obligatoires doivent être remplis.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var genre = cbbGenre.SelectedItem as Categorie;
            var public_ = cbbPublic.SelectedItem as Categorie;
            var rayon = cbbRayon.SelectedItem as Categorie;

            if (genre == null || public_ == null || rayon == null)
            {
                MessageBox.Show("Erreur lors de la récupération des catégories.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Récupération de tous les livres
                List<Livre> livres = controller.GetAllLivres();

                // Récupération du dernier ID
                string dernierID = "0";
                if (livres.Any())
                {
                    dernierID = livres.Max(l => l.Id);
                }

                // Incrémentation de l'ID (en assumant que l'ID est numérique)
                int nextId = int.Parse(dernierID) + 1;
                string nouvelId = nextId.ToString().PadLeft(3, '0'); // Format sur 10 caractères

                Livre livre = new Livre(
                    nouvelId,
                    txtTitre.Text.Trim(),
                    txtImage.Text.Trim(),
                    txtISBN.Text.Trim(),
                    txtAuteur.Text.Trim(),
                    txtCollection.Text.Trim(),
                    genre.Id.Trim(),
                    genre.Libelle.Trim(),
                    public_.Id.Trim(),
                    public_.Libelle.Trim(),
                    rayon.Id.Trim(),
                    rayon.Libelle.Trim()
                );

                if (controller.AjouterDocument("livre", livre))
                {
                    MessageBox.Show("Le livre a été ajouté avec succès.", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'ajout du livre.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
