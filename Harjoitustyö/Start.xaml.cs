

namespace Harjoitustyö;

public partial class Start : ContentPage
{
    private bool p1Turn = true;
    private int moveCount = 0;

    InfoStruct _info;

    IDispatcherTimer timer;
    int timerCount;
    public Start()
    {
        InitializeComponent();


    }

    private void ButtonClicked(object sender, EventArgs e) //Kaikki XAMLissa määritellyt pelialustan napit Noudattavat tämän tapahtuman logiikkaa.
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
            moveCount++;//Lasketaan vuoroja ja lisätään aina yksi
            if (FindWinner())// Suoritetaan funktio joka tarkistaa löytyykö kolmen suoraa
            {
                timer.Stop();//jos löytyi ajastin pysäytetään ja annetaan tekstilaatikko joka jertoo allaolevan.
                DisplayAlert("Winner!", "Congratulations!", "Exit");
            }
            else if(moveCount==9)//Jos kaikki siirrot on tehty eikä kolmen suoraa löytynyt peli pääättyy tasan
            {
                timer.Stop();// Taas ajastin pysäytetään ja annetaan alla oleva ilmoitu
                DisplayAlert("It's a tie", "GAME OVER", "Start over");
               
            }
            else
            {
                p1Turn = !p1Turn;// Tässä kohtaa muutetaan pelaajan vuoro asettamalla p1Turn arvo truesta falseksi ja jos se oli false muuttuu arvo trueksi, näin kontorloidaan pelaajien vuoroja
            }
        }
    }


    private bool FindWinner()
    {
        if (ThreeInaRow(rowone0, rowone1, rowone2) ||
            ThreeInaRow(rowtwo0, rowtwo1, rowtwo2) ||
            ThreeInaRow(rowthree0, rowthree1, rowthree2) ||
            // Vertikaaliset voittoehdot
            ThreeInaRow(rowone0, rowtwo0, rowthree0) ||
            ThreeInaRow(rowone1, rowtwo1, rowthree1) ||
            ThreeInaRow(rowone2, rowtwo2, rowthree2) ||
            // Diagonaalit
            ThreeInaRow(rowone0, rowtwo1, rowthree2) ||
            ThreeInaRow(rowone2, rowtwo1, rowthree0))
        {
            return true;
        }
        return false;
    }

    private bool ThreeInaRow(Button b1, Button b2, Button b3)
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

    private void Reset()
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
    }

    private void NewGame_Clicked(object sender, EventArgs e)
    {
        Reset();
        timer.Start();
    }


    public void WelcomeInfo(InfoStruct infoStruct) // Funktio suoritetaan MainPagella ja siellä tallennetaan tiedot Structiin jota hyödynnetään tällä sivulla.
    {
        _info = infoStruct;
        p1.Text = _info.Firstname; //Otetaan structiin tallennettu tieto ja tulostetaan se haluttuun kohtaan ohjelmassa.
    }

    
    public void SetOpponent(string opponentType) // Tuo tiedon vastustajan valinnasta ja kirjoittaa Labeliin valitun vastustajan
    {
        opponent.Text = opponentType;

    }


    public void StartTimer(int sec)// Ajastimen aloitus funktio, joka päivittää ajastinta myös sille omistettuun labeliin näkyville.
    {
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







}
