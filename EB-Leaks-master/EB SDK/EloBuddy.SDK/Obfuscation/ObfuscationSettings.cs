using System.Reflection;

// symbols
//[assembly: Obfuscation(Feature = "encrypt symbol names with password TRtpcnOpoJylAV4cCW2qGAxy5n3obcRauHZEHEGWCXNZvQwmfW0CXsAIcohB", Exclude = false)]
[assembly: Obfuscation(Feature = "Apply to type *: apply to member *: renaming", Exclude = true)]

// string encryption
[assembly: Obfuscation(Feature = "string encryption", Exclude = false)]

// resource encryption
[assembly: Obfuscation(Feature = "encrypt resources", Exclude = false)]
