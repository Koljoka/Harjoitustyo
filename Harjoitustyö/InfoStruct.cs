

namespace Harjoitustyö
{
    public struct InfoStruct
    {
        public string Firstname {  get; set; }// Vaatii GET ja SET jotta Json-tallennus onnistuu.
        public string Surname { get; set; }
        public string Birthyear { get; set; }
    }
}
