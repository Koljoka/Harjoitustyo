using System.Text.Json;

namespace Harjoitustyö
{
    public partial class MainPage : ContentPage
    {
        InfoStruct _player1;
        List<InfoStruct> _players = new List<InfoStruct> ();// Luodaan lista johon tallennetaan pelaajien tietoja Structista.

        public MainPage()
        {
            InitializeComponent();

            _players = LoadFromJson();
            _player1 = new InfoStruct();
        
            pickPlayer.ItemsSource = _players;

        }

        public List<InfoStruct> LoadFromJson()
        {
            string readFile = @"c:\temp\MyTest.json";
            string jsonString = File.ReadAllText(readFile);
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            List<InfoStruct> playerInfo = JsonSerializer.Deserialize<List<InfoStruct>>(jsonString, jsonOptions);

            return playerInfo;  
            //Lataa ensin vanha JSON lista , muuten tyhjä lista tallentaa päälle.
        }

        public void SaveToJson(List<InfoStruct> playerInfo)// Tehdään funktio joka suoritetaan napin painalluksen yhteydessä. Funktio tallentaa tiedot json muodossa tiedostoon.
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;// Hoitaa ääkköset oikein JSON-tallennukseen
        
            string fileName = @"c:\temp\MyTest.json";
            List<InfoStruct> existingPlayerInfo = new List<InfoStruct>();

            if(File.Exists(fileName)) //Luodaan funktion sisälle if ehto jolla tarkastetaan onko tiedosto jo olemassa, ja onko sillä sisältöä
            {
                string existingJsonString = File.ReadAllText(fileName);
                existingPlayerInfo = JsonSerializer.Deserialize<List<InfoStruct>>(existingJsonString, jsonOptions);
            }

            playerInfo = playerInfo.Where(p => p.Firstname != null && p.Surname != null && p.Birthyear != null).ToList();// Tarkistetaan nullit listasta

            existingPlayerInfo.AddRange(playerInfo);// Lisätään tiedot jo olemassa olevasta listasta uuteen listaan.

            var uniquePlayerInfo = existingPlayerInfo.Distinct().ToList(); // Karsitaan pois duplikaatit.

            string jsonString = JsonSerializer.Serialize(uniquePlayerInfo, jsonOptions);
            File.WriteAllText(fileName, jsonString);
         }

        private string selectedOpponent; //Luodaan muuttuja joka ottaa talteen vastustajan valinnan
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


        async void StartButton_Clicked(object sender, EventArgs e)// Napin painalluksesta siirrytään peli sivustolle, ja samassa käynnistettään ajastin ja suoritetaan yllä kuvattu funktio
        {
            _player1.Firstname = Fname.Text;   // Luetaan Entryistä tiedot Structiin
            _player1.Surname = Sname.Text;
            _player1.Birthyear = Byear.Text;    

            double sec = 0;

            Start start = new Start();
           
            start.StartTimer((int)sec);  //funktioita Start sivulla joiden avulla viedään tietoa MainPagelta Start-sivulle
            start.WelcomeInfo(_player1);
            start.SetOpponent(selectedOpponent);
            _players.Add(_player1);

            SaveToJson(_players);

                       
            await Navigation.PushAsync(start);
        }

       
    }
}