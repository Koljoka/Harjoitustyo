

namespace Harjoitustyö
{
    public struct InfoStruct//Tietue jonne kirjoitetaan tietoa pelin käyttäjistä ja toiminasta pelissä.
    {
        public string Firstname {  get; set; }// Vaatii GET ja SET jotta Json-tallennus onnistuu.
        public string Surname { get; set; }
        public string Birthyear { get; set; }
        public int Wins { get; set;}
        public int Losses { get; set;}
        public int Ties { get; set;}
        public int TimePlayed { get; set; }
    }
}
