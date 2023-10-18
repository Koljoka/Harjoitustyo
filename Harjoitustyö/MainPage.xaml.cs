
using System.Text.Json;

namespace Harjoitustyö
{
    public partial class MainPage : ContentPage
    {
        InfoStruct _player1;
        List<InfoStruct> _players = new List<InfoStruct> ();// Luodaan lista johon tallennetaan pelaajien tietoja InfoStructista(_player1).

        public const string FILENAME = "players.json"; //Nimetään json-tiedosto jonne kirjoitetaan, ja annetaan tallennuspolku. 
        string fileName = Path.Combine(FileSystem.Current.CacheDirectory, FILENAME);
        public MainPage()
        {
            InitializeComponent();

            if (File.Exists(fileName))
            {
                _players = LoadFromJson(); //Ladataan tiedot JSON-tiedostosta
            }
                _player1 = new InfoStruct();
        
            pickPlayer.ItemsSource = _players; //pickPlayer poimii tiedot _players listasta

            
        }

        public List<InfoStruct> LoadFromJson() //Haetaan tieto Json tiedostosta ja tallennetaan se List<InfoStruct> muodossa
        {
           
            string jsonString = File.ReadAllText(fileName);
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            List<InfoStruct> playerInfo = JsonSerializer.Deserialize<List<InfoStruct>>(jsonString, jsonOptions);

            return playerInfo;  
            //Lataa ensin vanha JSON lista , muuten tyhjä lista tallentaa päälle.
        }

        public void SaveToJson(List<InfoStruct> playerInfo)// Tehdään funktio, joka suoritetaan napin painalluksen yhteydessä. Funktio tallentaa tiedot json muodossa tiedostoon.
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;// Hoitaa ääkköset oikein JSON-tallennukseen


            string fileName = Path.Combine(FileSystem.Current.CacheDirectory, FILENAME);
            List<InfoStruct> existingPlayerInfo = new List<InfoStruct>();

            if(File.Exists(fileName)) //Luodaan funktion sisälle if-ehto jolla tarkastetaan onko tiedosto jo olemassa, ja onko sillä sisältöä
            {
                string existingJsonString = File.ReadAllText(fileName);
                existingPlayerInfo = JsonSerializer.Deserialize<List<InfoStruct>>(existingJsonString, jsonOptions);
            }

            playerInfo = playerInfo.Where(p => p.Firstname != null && p.Surname != null && p.Birthyear != null).ToList();// Tarkistetaan nullit listasta

            existingPlayerInfo.AddRange(playerInfo);// Lisätään tiedot jo olemassa olevasta listasta uuteen listaan.

            var uniquePlayerInfo = existingPlayerInfo.GroupBy(p => new { p.Firstname, p.Surname, p.Birthyear }).Select(g => g.OrderByDescending(p => p.Wins).First()).ToList();

            //var uniquePlayerInfo = existingPlayerInfo.Distinct().ToList(); // Karsitaan pois duplikaatit.

            string jsonString = JsonSerializer.Serialize(uniquePlayerInfo, jsonOptions);
            _players = uniquePlayerInfo;//_players-tietue korvataan uniquePlayerInfolla josta on poistettu duplikaatit.
            File.WriteAllText(fileName, jsonString);
         }

        private string selectedOpponent = null; //Luodaan muuttuja joka ottaa talteen vastustajan valinnan, alkuarvo asetetaan nulliksi, jotta voidaan myöhemmin tarkastaa, että käyttäjä on valinnut itselleen jonkun vastuksen
        private void Player2Button_Clicked(object sender, EventArgs e) // Napin painalluksella valitaan muuttujaan teksti, ja samalla värillä indikoidaan kummassa napissa valinta on aktiivisena.
        {
            selectedOpponent = "Player 2";
            Player2Button.BorderColor = Colors.Yellow;
            AIButton.BorderColor = Colors.Orange;
        }

        private void AIButton_Clicked(object sender, EventArgs e)
        {
            selectedOpponent = "AI";
            AIButton.BorderColor= Colors.Yellow;
            Player2Button.BorderColor= Colors.ForestGreen;
        }


        async void StartButton_Clicked(object sender, EventArgs e)// Napin painalluksesta siirrytään peli sivustolle.
        {
            _player1.Firstname = Fname.Text;   // Luetaan Entryistä tiedot Structiin
            _player1.Surname = Sname.Text;
            _player1.Birthyear = Byear.Text;
 
            

            //Tarkistetaan seuraavaksi, että jokaisessa entry-kentässä on tietoa.
            if (string.IsNullOrEmpty(_player1.Firstname) || string.IsNullOrEmpty(_player1.Surname) || string.IsNullOrEmpty(_player1.Birthyear))
            {
                await DisplayAlert("Error", "Fill every field with appropriate information", "OK");
                return;
            }
            //Tarkistetaan syntymävuosi vielä tarkemmin. Halutaan, että siihen voi syöttää vain tarkan vuoden numeerisena arvona.
            int year;
            if (!int.TryParse(_player1.Birthyear, out year) || (_player1.Birthyear.Length != 4))
            {
                await DisplayAlert("Error", "Give birth year in form of XXXX. Make sure you use only numbers.", "OK");
                return;
            }

            if (selectedOpponent == null)
            {
                await DisplayAlert("Error", "you must select an opponent", "OK");
                return;
            }

            var existingPlayer = _players.FirstOrDefault(p => p.Firstname == _player1.Firstname && p.Surname == _player1.Surname && p.Birthyear == _player1.Birthyear);//Verrataan _player1 tietoja _players listassa oleviin tietoihin. Mikäli löytyy samat tiedot = existingPlayer.

            if (existingPlayer.Firstname != null && existingPlayer.Surname != null && existingPlayer.Birthyear != null)// Päivtetään _player1 tiedot löydetyn pelaajan tiedoilla
            {
                _player1 = existingPlayer;
            }
            else
            {
                _players.Add(_player1);
            }
           

            Start start = new Start();
           
            start.StartTimer();  //funktioita Start sivulla joiden avulla viedään tietoa MainPagelta Start-sivulle
            
            start.SetOpponent(selectedOpponent);
            

            SaveToJson(_players);
            start.WelcomeInfo(_players, _player1);

            await Navigation.PushAsync(start);
        }

        private async void Scoreboard_Clicked(object sender, EventArgs e) // Napin painalluksesta siirrytään  ScoreBoard sivulle. scoreBoard.WelcomeInfo(_players) vie teidot myös sinne.
        {

            ScoreBoard scoreBoard = new ScoreBoard();

            scoreBoard.WelcomeInfo(_players);
            await Navigation.PushAsync(scoreBoard);

        }
    }
}