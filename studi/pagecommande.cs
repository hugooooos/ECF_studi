// Configuration menu (normalement vient du backend)
let currentMenu = {
    nom: 'Menu Mariage',
    prixUnitaire: 75,
    minPersonnes: 50
};

let formData = {
    nbPersonnes: 50
};

// ============================================
// NAVIGATION ENTRE ÉTAPES
// ============================================
function nextStep(stepNumber) {
    // Validation avant de passer à l'étape suivante
    const currentStepElement = document.querySelector('.step_content.active');
    const currentForm = currentStepElement.querySelector('form');
    
    if (currentForm && !currentForm.checkValidity()) {
        currentForm.reportValidity();
        return;
    }
    
    // Sauvegarder les données selon l'étape
    if (stepNumber === 2) {
        saveStep1Data();
    } else if (stepNumber === 3) {
        saveStep2Data();
        updateRecap();
    }
    
    // Changer l'étape active
    document.querySelectorAll('.step_content').forEach(step => step.classList.remove('active'));
    document.querySelectorAll('.progress_step').forEach(step => step.classList.remove('active'));
    
    document.getElementById('step' + stepNumber).classList.add('active');
    document.querySelectorAll('.progress_step')[stepNumber - 1].classList.add('active');
    
    // Scroll to top
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function prevStep(stepNumber) {
    nextStep(stepNumber);
}

// ============================================
// SAUVEGARDE DES DONNÉES
// ============================================
function saveStep1Data() {
    formData.nom = document.getElementById('nom').value;
    formData.prenom = document.getElementById('prenom').value;
    formData.email = document.getElementById('email').value;
    formData.telephone = document.getElementById('telephone').value;
    formData.adresse = document.getElementById('adresse').value;
    formData.codePostal = document.getElementById('code_postal').value;
    formData.ville = document.getElementById('ville').value;
    formData.dateLivraison = document.getElementById('date_livraison').value;
    formData.heureLivraison = document.getElementById('heure_livraison').value;
    formData.lieuEvenement = document.getElementById('lieu_evenement').value;
}

function saveStep2Data() {
    formData.nbPersonnes = parseInt(document.getElementById('nb_personnes').value);
    formData.remarques = document.getElementById('remarques').value;
}

// ============================================
// GESTION DU NOMBRE DE PERSONNES
// ============================================
function incrementPersonnes() {
    const input = document.getElementById('nb_personnes');
    input.value = parseInt(input.value) + 1;
    updatePrix();
}

function decrementPersonnes() {
    const input = document.getElementById('nb_personnes');
    const newValue = parseInt(input.value) - 1;
    if (newValue >= currentMenu.minPersonnes) {
        input.value = newValue;
        updatePrix();
    }
}

// Mise à jour auto quand on tape directement
document.addEventListener('DOMContentLoaded', function() {
    const nbPersonnesInput = document.getElementById('nb_personnes');
    if (nbPersonnesInput) {
        nbPersonnesInput.addEventListener('input', function() {
            if (parseInt(this.value) < currentMenu.minPersonnes) {
                this.value = currentMenu.minPersonnes;
            }
            updatePrix();
        });
    }
    
    // Set date minimum (aujourd'hui)
    const dateLivraison = document.getElementById('date_livraison');
    if (dateLivraison) {
        const today = new Date().toISOString().split('T')[0];
        dateLivraison.min = today;
    }
    
    // Initialiser le prix
    updatePrix();
});

// ============================================
// CALCUL DES PRIX
// ============================================
function updatePrix() {
    const nbPersonnes = parseInt(document.getElementById('nb_personnes').value) || currentMenu.minPersonnes;
    const prixUnitaire = currentMenu.prixUnitaire;
    
    // Calcul sous-total
    let sousTotal = nbPersonnes * prixUnitaire;
    
    // Réduction 10% si +5 personnes au-dessus du minimum
    let reduction = 0;
    let hasReduction = false;
    if (nbPersonnes >= (currentMenu.minPersonnes + 5)) {
        reduction = sousTotal * 0.10;
        sousTotal -= reduction;
        hasReduction = true;
    }
    
    // Mise à jour affichage
    document.getElementById('prix_unitaire').textContent = prixUnitaire.toFixed(2) + ' €';
    document.getElementById('display_nb_personnes').textContent = nbPersonnes;
    document.getElementById('sous_total_menu').textContent = sousTotal.toFixed(2) + ' €';
    
    // Afficher/masquer la réduction
    const reductionLine = document.getElementById('reduction_line');
    if (hasReduction) {
        reductionLine.style.display = 'flex';
        document.getElementById('montant_reduction').textContent = '- ' + reduction.toFixed(2) + ' €';
    } else {
        reductionLine.style.display = 'none';
    }
}

// ============================================
// CALCUL FRAIS DE LIVRAISON
// ============================================
function calculateLivraison(ville, codePostal) {
    // Simulation: 5€ à Bordeaux, sinon 5€ + 0.59€/km
    // Dans la vraie app, utiliser une API de calcul de distance (Google Maps Distance Matrix)
    
    const fraisBase = 5.00;
    
    if (ville.toLowerCase().includes('bordeaux') || codePostal.startsWith('33')) {
        return { frais: fraisBase, distance: 0, info: '(Bordeaux)' };
    } else {
        // Simulation: distance aléatoire entre 10 et 50 km
        const distance = 25; // À remplacer par calcul réel
        const fraisKm = distance * 0.59;
        return { 
            frais: fraisBase + fraisKm, 
            distance: distance, 
            info: `(${distance} km)`
        };
    }
}

// ============================================
// MISE À JOUR DU RÉCAPITULATIF
// ============================================
function updateRecap() {
    // Informations client
    document.getElementById('recap_nom').textContent = `${formData.prenom} ${formData.nom}`;
    document.getElementById('recap_email').textContent = formData.email;
    document.getElementById('recap_telephone').textContent = formData.telephone;
    
    // Livraison
    const adresseComplete = `${formData.adresse}, ${formData.codePostal} ${formData.ville}`;
    document.getElementById('recap_adresse').textContent = adresseComplete;
    
    const dateFormatted = new Date(formData.dateLivraison).toLocaleDateString('fr-FR', {
        day: 'numeric',
        month: 'long',
        year: 'numeric'
    });
    document.getElementById('recap_date_heure').textContent = `${dateFormatted} à ${formData.heureLivraison}`;
    document.getElementById('recap_lieu').textContent = formData.lieuEvenement || 'Non précisé';
    
    // Menu
    document.getElementById('recap_menu').textContent = currentMenu.nom;
    document.getElementById('recap_nb_personnes').textContent = `${formData.nbPersonnes} personnes`;
    
    // Calculs
    const prixUnitaire = currentMenu.prixUnitaire;
    let sousTotal = formData.nbPersonnes * prixUnitaire;
    let reduction = 0;
    let hasReduction = false;
    
    if (formData.nbPersonnes >= (currentMenu.minPersonnes + 5)) {
        reduction = sousTotal * 0.10;
        sousTotal -= reduction;
        hasReduction = true;
    }
    
    const livraison = calculateLivraison(formData.ville, formData.codePostal);
    const total = sousTotal + livraison.frais;
    const acompte = total * 0.30;
    
    // Affichage tarification
    document.getElementById('final_nb_personnes').textContent = formData.nbPersonnes;
    document.getElementById('final_prix_unitaire').textContent = prixUnitaire.toFixed(2);
    document.getElementById('final_sous_total').textContent = (formData.nbPersonnes * prixUnitaire).toFixed(2) + ' €';
    
    const finalReductionLine = document.getElementById('final_reduction_line');
    if (hasReduction) {
        finalReductionLine.style.display = 'flex';
        document.getElementById('final_reduction').textContent = '- ' + reduction.toFixed(2) + ' €';
    } else {
        finalReductionLine.style.display = 'none';
    }
    
    document.getElementById('distance_info').textContent = livraison.info;
    document.getElementById('final_livraison').textContent = livraison.frais.toFixed(2) + ' €';
    document.getElementById('final_total').textContent = total.toFixed(2) + ' €';
    document.getElementById('final_acompte').textContent = acompte.toFixed(2) + ' €';
}

// ============================================
// SÉLECTION MENU (MODAL)
// ============================================
function openMenuSelector() {
    document.getElementById('menuModal').classList.add('active');
}

function closeMenuSelector() {
    document.getElementById('menuModal').classList.remove('active');
}

function selectMenu(nom, prix, minPersonnes) {
    currentMenu = {
        nom: nom,
        prixUnitaire: prix,
        minPersonnes: minPersonnes
    };
    
    // Mise à jour affichage
    document.getElementById('selected_menu_name').textContent = nom;
    document.getElementById('nb_personnes').value = minPersonnes;
    document.getElementById('nb_personnes').min = minPersonnes;
    document.getElementById('min_personnes_help').textContent = `Minimum : ${minPersonnes} personnes pour ce menu`;
    
    updatePrix();
    closeMenuSelector();
}

// ============================================
// SOUMISSION DE LA COMMANDE
// ============================================
function submitOrder(event) {
    event.preventDefault();
    
    const cgvAccepted = document.getElementById('accept_cgv').checked;
    if (!cgvAccepted) {
        alert('Veuillez accepter les conditions générales de vente pour continuer.');
        return;
    }
    
    // Dans une vraie app, envoyer les données au backend ici
    console.log('Commande soumise:', {
        ...formData,
        menu: currentMenu,
        total: calculateTotal()
    });
    
    // Afficher confirmation
    document.getElementById('confirm_email').textContent = formData.email;
    document.getElementById('confirmModal').classList.add('active');
}

function calculateTotal() {
    let sousTotal = formData.nbPersonnes * currentMenu.prixUnitaire;
    
    if (formData.nbPersonnes >= (currentMenu.minPersonnes + 5)) {
        sousTotal *= 0.90; // -10%
    }
    
    const livraison = calculateLivraison(formData.ville, formData.codePostal);
    return sousTotal + livraison.frais;
}

// ============================================
// MENU BURGER
// ============================================
function toggleMenu() {
    const menu = document.querySelector('.menu_deroulant');
    menu.classList.toggle('active');
}

// Fermer modal en cliquant en dehors
window.onclick = function(event) {
    const menuModal = document.getElementById('menuModal');
    const confirmModal = document.getElementById('confirmModal');
    
    if (event.target === menuModal) {
        closeMenuSelector();
    }
    if (event.target === confirmModal) {
        confirmModal.classList.remove('active');
    }
}