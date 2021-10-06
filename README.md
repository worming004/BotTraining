# Simple example d'un bot discord, pour un pote et la science

## doc

https://swimburger.net/blog/azure/how-to-create-a-discord-bot-using-the-dotnet-worker-template-and-host-it-on-azure-container-instances

## commandes

Il ne répond que à 'Hello' et 'Help' dans un channel au nom de 'bot-training'.


## luis tiers

J'utilise actuellement le free tiers Luis https://azure.microsoft.com/en-us/pricing/details/cognitive-services/language-understanding-intelligent-services/
Simplement 10 000 requêtes par mois. Suffisant pour un petit bot

## bot suffixe

malheureseument, je n'ai pas entrainé l'ia pour exclure les messages normaux.
une idée et d'assurer que la commande est pour le bot en ajouter le suffixe "bot [command]". Cette solution n'est pas déployé, mais un code d'exemple existe ici https://github.com/worming004/BotTraining/commit/d5d53bd1e3bc93871f0bc481e7feb35fa8e5e4fd

## version sans IA

Luis apporte un avantage certain, mais si vous êtes intéressé par la version sans Luis, vous pouvez jeter un oeil sur le git tag [**ia-less**](https://github.com/worming004/BotTraining/tree/ia-less)