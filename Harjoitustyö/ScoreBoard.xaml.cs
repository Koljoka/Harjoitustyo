namespace Harjoitustyö;

public partial class ScoreBoard : ContentPage
{
    List<InfoStruct> _infoList;
	public ScoreBoard()
	{
		InitializeComponent();

        
	}

    public void WelcomeInfo(List<InfoStruct> infoStruct) // Funktio suoritetaan MainPagella ja siellä tallennetaan tiedot Structiin jota hyödynnetään tällä sivulla.
    {
        _infoList = infoStruct;// Tuodaan ScoreBoard-sivulle kaikki Listalla olevat tiedot
        playersList.ItemsSource = _infoList;
    }


}