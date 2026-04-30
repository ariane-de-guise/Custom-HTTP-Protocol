**Architecture décrite rapidement dans la vidéo, mais aussi dans la documentation XML du projet.**
[Vidéo](https://youtu.be/Cy-EbvorUQ0)

**Instructions**
Pour lancer le serveur, 
1. Tapez dans votre terminal `cd tp2-ariane-de-guise/tp2/server`
2. Tapez dans votre terminal `dotnet run`

Pour tester le serveur,
- Dans un navigateur : **Tests généraux + redirection (303)**
  1. Dans la barre de recherche, taper `localhost:8088`
  2. Ensuite, ajouter un message en remplissant le input et en cliquant sur le bouton `Envoyer`
  3. On se fait rediriger automatiquement vers `localhost:8088/api/messages`
  4. Cliquer sur le bouton `View` du message qui vient d'être créé.
  5. Modifier le message et cliquer sur le bouton `Edit`
  6. On se fait rediriger automatiquement vers `localhost:8088/api/messages`
  7. Cliquer sur le bouton `View` du message qui vient d'être modifié.
  8. Cliquer sur le bouton `Delete`
  9. Cliquer sur le bouton `Confirm`
  10. On se fait rediriger automatiquement vers `localhost:8088/api/messages` où il ne devrait rester aucun message
      
- Dans netcat : **Test 405, 200, 201**
	1. Tapez `nc 127.0.0.1 8088` dans votre terminal
	2. Tapez `PATCH / HTTP/1.1` puis appuyez sur `Enter`
 	3. Vous devriez recevoir `HTTP/1.1 405 Method Not Allowed`
  4. Appuyez sur `Enter`
  5. Tapez `nc 127.0.0.1 8088` dans votre terminal
  6. Tapez `POST /api/messages HTTP/1.1` puis appuyez sur `Enter`
  7. Tapez `Host: localhost:8088` puis appuyez sur `Enter`
  8. Tapez `Content-length: 5` puis appuyez sur `Enter`
  9. Appuyez sur `Enter`
  10. Tapez `Hello` puis appuyez sur `Enter`
  11. Vous devriez recevoir `HTTP/1.1 201 Created`
  12. Appuyez sur `Enter`
  13. Tapez `nc 127.0.0.1 8088` dans votre terminal
  6. Tapez `GET /api/messages HTTP/1.1` puis appuyez sur `Enter`
  7. Tapez `Host: localhost:8088` puis appuyez sur `Enter`
  8. Appuyez sur `Enter`
  9. Vous devriez recevoir `HTTP/1.1 200 Ok` avec un JSON [ { id="1", content="Hello" } ]
  10. Appuyez sur `Enter`

- Avec le client configuré : **Test 404**
  1. Tapez dans votre terminal `cd tp2-ariane-de-guise/tp2/client`
  2. Tapez dans votre terminal `dotnet run`
  3. Tapez dans votre terminal `1`
  4. Tapez dans votre terminal `6`
  5. Vous derviez recevoir `HTTP/1.1 404 Not Found`
  6. Appuyez sur `Enter`
  7. Tapez dans votre terminal `Q`
