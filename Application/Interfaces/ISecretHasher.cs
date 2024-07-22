namespace Application.Interfaces;

public interface ISecretHasher
{
    string Hash(string input);
    bool Verify(string input, string hashString);
}


