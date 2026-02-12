<?php
session_start();

// Vérifier que l'utilisateur est connecté
if (!isset($_SESSION['user_id'])) {
    header("Location: connexion.html");
    exit();
}

// Vérifier que le menu existe
$menuId = intval($_POST['menu_id']);
// Requête SQL pour vérifier...

// Créer la commande...
?>