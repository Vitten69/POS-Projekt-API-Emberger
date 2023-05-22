# Dokumentation ArticleAPI
### Inhaltsverzeichnis
*
<br>

## Software Architektur
```mermaid
graph LR
A(Blazor Website) -- gets Data --> C
B(WPF App) -- gets and changes Data --> C
C(Spring Server with API) -- stores Data and gets --> D
D(MongoDB)
```
Die zwei Clients greifen ihre Daten vom Server über die API ab und können auch Anfragen an die API senden. Der Server regelt alle Ab- und Abfragen, die über die API anfallen. Außerdem kommuniziert er mit der Datenbank, sodass die Daten auch persistent gespeichert werden.
<br>
## Beschreibung Software
In der Software geht es um die Erstellung, Verwaltung und Ansicht von Artikeln. Jeder Artikel hat eine, von MongoDB vergebene, ID, einen Titel, einen Inhalt, eine Kategorie, einen Publisher/Autor und einen Erstellungszeitpunkt (Datum und Uhrzeit). Die Daten werden über eine API bereitgestellt und man kann sie von einer WPF-Anwendung und von einer Website erreichen. 
<br>

### WPF App
Die WPF App kann sozusagen als Admin-Panel betrachtet werden, da sie alle CRUD-Operationen (Create, Read, Update, Delete) abbildet, die Daten aber relativ roh und ohne Zusammenhang zwischen den Artikeln dargestellt werden. 
<br>

**POST**
Das Hinzufügen eines Artikels wird mittels HttpClient geregelt. Dieser schickt ein POST-Request mit einem Artikel als JSON Objekt im Body an die API. 
```c#
HttpClient client = new HttpClient();
Article a = new Article();

string json = JsonConvert.SerializeObject(a);
StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

HttpResponseMessage response = client.PostAsync(baseApiURL, content).Result;
```
<br>

**GET** 
Um die Artikel zu bekommen und dann anzeigen zu können wird ein GET-Request an die API geschickt. Anschließend werden die Daten im JSON Format in eine Liste von Artikel Objekten umgewandelt.
```c#
response = client.GetStringAsync(baseApiURL).Result;
articles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Article>>(response);
```
<br>

**PUT/UPDATE**
Dass man Artikel verändern kann, muss man ein PUT-Request an die API schicken. Dieses enthält, wie schon beim POST-Request, das neue, veränderte Artikel Objekt im JSON Format im Body und die ID des Artikels in der URL. 
```c
HttpResponseMessage response = client.PutAsync(URL, content).Result;
```
<br>

**DELETE**
Um Artikel zu löschen kommt ein DELETE-Request zum Einsatz, bei dem die ID des Artikels in der URL mitgegeben werden muss. 
```c
HttpResponseMessage message = await client.DeleteAsync(URL);
```
<br>

### Blazor Website
Auf der Website kann man die Artikel schöner formatiert als bei der WPF App betrachten. Außerdem gibt es noch die Funktion die Artikel nach verschiedenen Parametern zu Gruppieren. Man kann sich also alle Artikel eines bestimmen Publishers anzeigen lassen oder alle Artikel einer Kategorie abrufen. 

Von den CRUD-Operationen wird auf der Website "Read" abgebildet. Da die Website mit Blazor, ein Framework, das auf C# aufbaut, erstellt wurde, ist der Code rund um das GET-Request der gleiche wie bei der WPF Anwendung. 
<br>
## Beschreibung der API

<details>
 <summary><code>POST</code> <code><b></b></code> <code>/articles</code></summary>

##### Parameters

> no Parameters

<br>

##### RequestBody (required)
```yaml
{
"title":  "titleneu",
"content":  "content1",
"publisher":  "Anonymous",
"category":  "natur"
}
```
<br>

##### Responses

> http code | response|
> -|-
> `200`        | `Configuration created successfully`
> `400`        | `{"code":"400","message":"Bad Request"}` 
</details>
<br>

<details>
 <summary><code>GET</code> <code><b></b></code> <code>/articles</code></summary>

##### Parameters

> no Parameters

<br>

##### RequestBody
>None

<br>

##### Responses

> http code | response|
> -|-
> `200`        | `Successful Operation`
> `400`        | `{"code":"400","message":"Bad Request"}` 
> `404`        | `Not found`
</details>
<br>

<details>
 <summary><code>PUT</code> <code><b></b></code> <code>/articles/{id}</code></summary>

##### Parameters

> no Parameters

<br>

##### RequestBody (required)
```yaml
{
"id": "64666ae5d0ee1801cb2260ed"
"title":  "titleneu",
"content":  "content1",
"publisher":  "Anonymous",
"category":  "natur"
}
```
<br>

##### Responses

> http code | response|
> -|-
> `200`        | `Configuration created successfully`
> `400`        | `{"code":"400","message":"Bad Request"}` 
> `404`        | `Not found` 
</details>
<br>
<details>
  <summary><code>DELETE</code> <code>/articles/{id}</code></summary>

##### Parameters

> no Parameters

<br>

##### RequestBody
> None
> 
<br>

##### Responses

> http code | response|
> -|-
> `400`        | `{"code":"400","message":"Bad Request"}` 
> `404`        | `Not Found`
</details>
