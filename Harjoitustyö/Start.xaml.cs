

using Microsoft.Maui.Storage;
using System.Text.Json;

namespace Harjoitustyö;

public partial class Start : ContentPage
{
    public const string FILENAME = "players.json";
    string fileName = Path.Combine(FileSystem.Current.CacheDirectory, FILENAME);

    private bool p1Turn = true;
    private int moveCount = 0;

    InfoStruct _player;
    List<InfoStruct> _infoList;

    IDispatcherTimer timer;
    int timerCount;
    public Start()
    {
        InitializeComponent();
        TurnIndikation();//Suoritetaan vuorossa olevan pelaajan osoittava funktio

    }

    private async void ButtonClicked(object sender, EventArgs e) //Kaikki XAMLissa määritellyt pelialustan napit Noudattavat tämän tapahtuman logiikkaa.
    {
        var button = sender as Button; //Tallennetaan button muuttujaan tietoa klikeistä

        if (button.Text == null)//Jos nappulaan on jo kirjoitettu "tekstiä", ei tapahdu mitään
        {
            if (p1Turn) // jos p1Turnin arvo on true kirjoitetaan X
            {
                button.Text = "X";
            }
            else
            {
                button.Text = "O";//jos taas ei niin silloin se on false ja kirjoitetaan O
            }
            moveCount++;     //Lasketaan vuoroja ja lisätään aina yksi
            if (FindWinner())// Suoritetaan funktio joka tarkistaa löytyykö kolmen suoraa
            {
                timer.Stop();//jos löytyi ajastin pysäytetään ja annetaan tekstilaatikko joka kertoo allaolevan.

                if (p1Turn) // Käydään voittajavaihtoehdot läpi ja ilmoitetaan se asianmukaisella tekstillä DisplayAlertissa
                {
                    await DisplayAlert($"{_player.Firstname} is the WINNER!", "Congartulations!", "Exit" );
                    
                    _player.Wins += 1;
                }    
            else if (opponent.Text == "AI") 
            {
                    await DisplayAlert("AI is the WINNER.", "Better luck next time", "Exit");
                    _player.Losses += 1;  
                }
            else
            {
                    await DisplayAlert("Player 2 is the WINNER", "Congratulations!", "Exit");
                    _player.Losses += 1;
                }
            }
            else if(moveCount==9)//Jos kaikki siirrot on tehty eikä kolmen suoraa löytynyt peli pääättyy tasan
            {
                timer.Stop();// Taas ajastin pysäytetään ja annetaan alla oleva ilmoitu
                await DisplayAlert("It's a tie", "GAME OVER", "Start over");
                _player.Ties += 1;
            }
            else
            {
                p1Turn = !p1Turn;// Tässä kohtaa muutetaan pelaajan vuoro asettamalla p1Turn arvo truesta falseksi, ja jos se oli false, muuttuu arvo trueksi. Näin kontorloidaan pelaajien vuoroja
                TurnIndikation();
            }
            if (!p1Turn && opponent.Text == "AI")
            {
                await Task.Delay(TimeSpan.FromSeconds(new Random().NextDouble() * 1.5 + 0.5));// Jos todetaan, että pelaajaksi on määritelty AI ja ei ole p1 vuoro, odotetaan hetki, ja sen jälkeen suoritetaan tekoälyn toimintaa ohjaava funktio.

                AITurn();
                
            }
        }
        if (FindWinner() || moveCount == 9)
        {
            var existingPlayer = _infoList.FirstOrDefault(p => p.Firstname == _player.Firstname && p.Surname == _player.Surname);//existingPlayer muuttujaan tallennetaan tietue jossa edellä mainitut tiedot täsmäävät.

            int index = _infoList.IndexOf(existingPlayer);//indexiin tallennetaan _infoLististä Listan  mukainen järjestysnumero.
            if (index != -1)
            {
                _infoList[index] = _player; // existingPlayerin järjestysnumeron mukaiseen indexiin tallennetaan _player-structin tiedot.
            }


            
            SaveToJson(_infoList);//Tallennetaan päivitetty lista

        }

    }

    public void SaveToJson(List<InfoStruct> playerInfo)// Tehdään funktio, joka suoritetaan napin painalluksen yhteydessä. Funktio tallentaa tiedot json muodossa tiedostoon.
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
        jsonOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;// Hoitaa ääkköset oikein JSON-tallennukseen

       
        string fileName = Path.Combine(FileSystem.Current.CacheDirectory, FILENAME);
        List<InfoStruct> existingPlayerInfo = new List<InfoStruct>();

        if (File.Exists(fileName)) //Luodaan funktion sisälle if ehto jolla tarkastetaan onko tiedosto jo olemassa, ja onko sillä sisältöä
        {
            string existingJsonString = File.ReadAllText(fileName);
            existingPlayerInfo = JsonSerializer.Deserialize<List<InfoStruct>>(existingJsonString, jsonOptions);
        }

        playerInfo = playerInfo.Where(p => p.Firstname != null && p.Surname != null && p.Birthyear != null).ToList();// Tarkistetaan nullit listasta

        existingPlayerInfo.AddRange(playerInfo);// Lisätään tiedot jo olemassa olevasta listasta uuteen listaan.

        var uniquePlayerInfo = existingPlayerInfo.GroupBy(p => new { p.Firstname, p.Surname, p.Birthyear }).Select(g => g.OrderByDescending(p => p.Wins).First()).ToList();// Poistetaan dublikaatit, tallennetaan vain tietue jossa voittojen/tapioiden/tasapelien arvot ovat korkeimmat.

        string jsonString = JsonSerializer.Serialize(uniquePlayerInfo, jsonOptions);
        File.WriteAllText(fileName, jsonString);
    }



    private bool FindWinner()//Funktio tarkistaa löytyyko kolmen suoraa jostakin
    {
        if (ThreeInaRow(rowone0, rowone1, rowone2) ||// Ensin käydään läpi vaakatasossa olevat rivit
            ThreeInaRow(rowtwo0, rowtwo1, rowtwo2) ||
            ThreeInaRow(rowthree0, rowthree1, rowthree2) ||
            
            ThreeInaRow(rowone0, rowtwo0, rowthree0) ||//Seuraavaksi pystyrivit
            ThreeInaRow(rowone1, rowtwo1, rowthree1) ||
            ThreeInaRow(rowone2, rowtwo2, rowthree2) ||
            
            ThreeInaRow(rowone0, rowtwo1, rowthree2) ||//Kolmantena viisto vaihtoehdot
            ThreeInaRow(rowone2, rowtwo1, rowthree0))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    private bool ThreeInaRow(Button b1, Button b2, Button b3)//Kun löydetään kolmen suora, tämä funktio värjää kyseisen suoran nappuloiden ääriviivat keltaisiksi, ja osoittaa sillä tavoin voittorivin. Funktiota hyödynnetaan FindWinner funktion sisällä tarkistuksen yhteydessä
    {
        if (b1.Text != null && b1.Text == b2.Text && b2.Text == b3.Text)
        {
            b1.BorderColor = Colors.Yellow;  
            b2.BorderColor = Colors.Yellow;
            b3.BorderColor = Colors.Yellow;
            return true;
        }
        return false;
    }

    private void Reset()//Tämä funktio suoritetaan aina kun halutaan nollata peli. Siinä käydään foreachilla läpi kaikki "pelilaudan" painikkeet ja palautetaan nappulat samaan tilaan jossa ne olivat pelin alkaessa. Tämän lisäksi nollataan vuorolaskuri ja määrätään aloittavaksi pelaajaksi p1.
    {
        foreach (var child in Table.Children)
        {
            if (child is Button button && button != NewGame)
            {
                button.Text = null;
                button.BorderColor = Colors.LightBlue;
            }
        }
        p1Turn = true;
        moveCount = 0;

        TurnIndikation();
    }

    private void NewGame_Clicked(object sender, EventArgs e)//New Game nappi aloittaa uuden pelin suorittamalla reset-funktion ja käynnistämällä ajastimen uudelleen.
    {
        Reset();
        StartTimer();
    }


    public void WelcomeInfo(List<InfoStruct> infoStruct, InfoStruct player) // Funktio suoritetaan MainPagella ja siellä tallennetaan tiedot Structiin jota hyödynnetään tällä sivulla.
    {
        _infoList = infoStruct;// Tuodaan start-sivulle kaikki Listalla olevat tiedot
        _player = player;
        p1.Text = player.Firstname; //Otetaan structiin tallennettu tieto ja tulostetaan se haluttuun kohtaan ohjelmassa.
    }

    
    public void SetOpponent(string opponentType) // Tuo tiedon vastustajan valinnasta ja kirjoittaa Labeliin valitun vastustajan
    {
        opponent.Text = opponentType;

    }


    public void StartTimer()// Ajastimen aloitus funktio, joka päivittää ajastinta myös sille omistettuun labeliin näkyville.
    {

        int sec = 0;    

        timerCount = sec;
        TimerL.Text = sec.ToString();
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (s, e) => DisplayTimer();
        timer.Start();

    }

    void DisplayTimer()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            timerCount++;
            TimerL.Text = timerCount.ToString();
        });
    }






    private void AITurn()//Tekoälyn toimintaa ohjaava funktio
    {
        
        List<Button> availableButtons = new List<Button>();// Luodaa uusi lista johon luetaan tiedoksi kaikki "vapaat napit/kentät"

        foreach (var child in Table.Children)//Foreach käy läpi Gridin nimeltä Table ja sen nappi-elementit (paitsi NewGame napin)
        {
            if (child is Button button && button != NewGame && button.Text == null)
            {
                availableButtons.Add(button);
            }
        }

        
        if (availableButtons.Count > 0)// Jos löytyy tyhjiä ruutuja enemmän kuin 0, kohteeksi valitaan näistä tyhjistä randomilla joku. Lisätään lopuksi siirtolaskuriin yksi siirto lisää
        {
            var targetButton = availableButtons[new Random().Next(availableButtons.Count)];
            targetButton.Text = "O";
            moveCount++;

            
            if (FindWinner())// Suoritetaan funktio joka tarkastaa löytyikö kolmen suoraa
            {
                timer.Stop();
                DisplayAlert("AI is the WINNER.", "Better luck next time", "Exit");//Jos löytyi ja vuoro oli AI:n ilmoietaan tekoäly voittajaksi
            }
            else if (moveCount == 9)
            {
                timer.Stop();
                DisplayAlert("It's a tie", "GAME OVER", "Start over");//Jos taas liikelaskuri tuli täyteen eikä löytynyt kolmen suoraa tulos on tasapeli
            }
            p1Turn = true;
            TurnIndikation();
        }
    }




    private void TurnIndikation() // Funktio jonka tehtävänä on värjätä vuorossa olevan pelaajan nimi keltaiseksi.
    {
        if (p1Turn)
        {
            p1.TextColor = Colors.Yellow;

            opponent.TextColor = Colors.Red;
        }
        else
        {
            opponent.TextColor = Colors.Yellow;

            p1.TextColor = Colors.CornflowerBlue;
        }
    }



}
