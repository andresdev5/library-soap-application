﻿using LibrarySoapService;

namespace LibraryClient.Models
{
    public class Author
    {
        private long _id;
        private string _firstname = "";
        private string _lastname = "";
        private string _pseudonym = "";
        private DateTime _birthdate;
        private string _displayName;

        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Firstname
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        public string Lastname
        {
            get { return _lastname; }
            set { _lastname = value; }
        }

        public string Pseudonym
        {
            get { return _pseudonym; }
            set { _pseudonym = value; }
        }

        public DateTime Birthdate
        {
            get { return _birthdate; }
            set { _birthdate = value; }
        }

        public string DisplayName
        {
            get { return _pseudonym != null && _pseudonym != "" ? _pseudonym : $"{_firstname} {_lastname}"; }
        }

        public Author() {}

        public Author(long id, string firstname, string lastname, string pseudonym, DateTime birthdate)
        {
            _id = id;
            _firstname = firstname;
            _lastname = lastname;
            _pseudonym = pseudonym;
            _birthdate = birthdate;
        }

        public Author(AuthorModel model)
        {
            _id = model.id;
            _firstname = model.firstname;
            _lastname = model.lastname;
            _pseudonym = model.pseudonym;
            _birthdate = model.birthdate;
        }

        public string CompleteDisplayName
        {
            get
            {
                var values = new List<string>
                {
                    _id.ToString() + " -",
                    _firstname,
                    _lastname,
                    _pseudonym,
                };

                    return string.Join(" ", values.Where(v => !string.IsNullOrEmpty(v)));
            }
        }

        public override string ToString() => CompleteDisplayName;
    }
}
