# Jeu - Projet de départ

```
À faire : Écrire une courte description du projet ici. Remplacer le titre de ce document par le nom de votre jeu.

```

```
À faire : Mettre à jour les liens dans ce document vers votre dépôt. 
Par exemple : 
   https://gitlab.com/csfpwmjv/projet-synthese/starter-game.git
```

## Démarrage rapide

Ces instructions vous permettront d'obtenir une copie opérationnelle du projet sur votre machine à des fins de développement.

### Prérequis

* [Git](https://git-scm.com/downloads) - Système de contrôle de version. Utilisez la dernière version.
* [Rider](https://www.jetbrains.com/rider/) ou [Visual Studio](https://www.visualstudio.com/fr/) - IDE. Vous pouvez utiliser 
  également n'importe quel autre IDE: assurez-vous simplement qu'il supporte les projets Unity.
* [Unity 2019.2.1f1](https://unity3d.com/fr/get-unity/download/) - Moteur de jeu. Veuillez utiliser **spécifiquement cette 
  version.** Attention à ne pas installer Visual Studio si vous avez déjà un IDE. Vous pouvez aussi utiliser Unity Hub pour 
  effectuer l'installation.

**Attention!** Actuellement, seul le développement sur Windows est supporté.

### Compiler une version de développement

Tout d'abord, vérifiez que `git` est présents dans votre variable d'environnement `PATH`.

```
git --version
```

Ensuite, clonez le projet **en vous assurant qu'il n'y a pas d'espace dans le chemin vers le dossier de destination.**

```
cd /folder/with_no_space/
git clone https://gitlab.com/csfpwmjv/projet-synthese/starter-game.git
```

Avant d'ouvrir le projet dans Unity, exécutez le script `GenerateCode.bat`. Ce dernier va générer du code C # (principalement 
des constantes). Cela pourrait prendre un certain temps.

```
cd game-starter
./GenerateCode.bat
```

Notez que vous aurez à régénérer le code régulièrement.

### Tester un version stable ou de développement

Ouvrez le projet dans Unity. Ensuite, allez dans `File > Build Settings…` et compilez une version Windows X64.

Si vous rencontrez un bogue, vous êtes priés de le [signaler](https://gitlab.com/csfpwmjv/projet-synthese/starter-game/issues/new?issuable_template=Bug). 
Veuillez fournir une explication détaillée de votre problème avec les étapes pour reproduire le bogue. Les captures d'écran et 
les vidéos jointes sont les bienvenues.

## Mettre à jour les dépendances

### DoTween

Téléchargez la dernière version de [DoTween](http://dotween.demigiant.com/download.php). Remplacez le contenu du dossier ```Assets/Libraries/DoTween```
par le contenu du fichier ```zip``` que vous aurez téléchargé.

Il est possible que vous ayez à reconfigurer ```DoTween```. Pour ce faire, supprimez le fichier ```Assets/Resources/DOTweenSettings.asset```.

### XInput

Téléchargez la dernière version de [XInputDotNet](https://github.com/speps/XInputDotNet/releases). Vous obtiendrez un
package Unity que vous pourrez importer. Une fois importé, supprimez les fichiers en trop et conservez que les fichiers
suivants :

* XInputDotNetPure.dll
* XInputInterface.dll (pour x86)
* XInputInterface.dll (pour x86_64)

Prenez ces DLL dans leurs dossiers respectifs (voir ```Assets/Libraries/XInput/Plugins```).

### SQLite

Allez sur le [site de SQLite](https://www.sqlite.org/index.html) et identifiez la dernière version. Ensuite, remplacez
les derniers chiffres de ces deux URL par les numéros de version, sans les points. Par exemple, pour ```3.25.2```, 
vous obtenez ```3250200```.

```
https://www.sqlite.org/2018/sqlite-dll-win32-x86-3250200.zip

https://www.sqlite.org/2018/sqlite-dll-win64-x64-3250200.zip
```

Prenez les deux DLL et placez-les dans leurs dossiers respectifs (voir ```Assets/Libraries/SqLite/Plugins```).

## Contribuer au projet

Veuillez lire [CONTRIBUTING.md](CONTRIBUTING.md) pour plus de détails sur notre code de conduite.

## Auteurs

```
À faire : Ajoutez vous noms ici ainsi que le nom de tout artiste ayant participé au projet 
(avec lien vers leur portfolio s'il existe).

Inscrivez aussi, en détail, ce sur quoi chaque membre de l'équipe a principalement travaillé.
```

* **Benjamin Lemelin** - *Programmeur*
  * Extensions sur le moteur Unity pour la recherche d'objets et de composants. Générateur de constantes. Gestionnaire de
    chargement des scènes.
* **Prénom Nom** - *Programmeur*
* **Prénom Nom** - *Programmeur*
* **Prénom Nom** - *Programmeur*
* **Prénom Nom** - *Programmeur*
* **Prénom Nom** - *Concepteur sonore*
* **Prénom Nom** - *Artiste 2D et Artiste UI*

## Remierciements

```
À faire : Remercier toute personne ayant contribué au projet, mais qui n'est pas un auteur.
```

* Tyler Coles - Pour [son guide](https://ornithoptergames.com/how-to-set-up-sqlite-for-unity/) d'intégration de SQLite dans Unity, dont l'implémentation dans ce projet fut fortement inspirée.