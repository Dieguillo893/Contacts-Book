using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Contact
{
    public string FirstName;
    public string LastName;
    public string Phone;
    public string Email;

    public override string ToString()
    {
        return $"Name: {FirstName} {LastName}, Phone: {Phone}, Email: {Email}";
    }

    public bool IsDuplicateOf(Contact other)
    {
        return FirstName.ToLower() == other.FirstName.ToLower() &&
               LastName.ToLower() == other.LastName.ToLower();
    }
}

class ContactsBook
{
    static List<Contact> contacts = new List<Contact>();
    static bool changesMade = false;

    static void Main()
    {
        while (true)
        {
            ShowMainMenu();
            string option = Console.ReadLine().Trim();
            Console.Clear();
            switch (option)
            {
                case "1": LoadContacts(); break;
                case "2": StoreContacts(); break;
                case "3": ShowContacts(); break;
                case "4": AddContact(); break;
                case "5": EditContact(); break;
                case "6": DeleteContact(); break;
                case "7": MergeDuplicates(); break;
                case "8": ExitApplication(); return;
                default: Console.WriteLine("Invalid option.\n"); break;
            }
        }
    }

    static void ShowMainMenu()
    {
        Console.WriteLine("### Contacts Book ###");
        Console.WriteLine("1. Load contacts from file");
        Console.WriteLine("2. Store contacts to file");
        Console.WriteLine("3. Show contacts");
        Console.WriteLine("4. Add new contact");
        Console.WriteLine("5. Edit existing contact");
        Console.WriteLine("6. Delete contact");
        Console.WriteLine("7. Merge duplicate contacts");
        Console.WriteLine("8. Exit");
        Console.Write("Select an option: ");
    }

    static void LoadContacts()
    {
        Console.Write("### Load Contacts ###\nFilename: ");
        string filename = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(filename)) return;

        if (!File.Exists(filename))
        {
            Console.WriteLine("File not found.\n");
            return;
        }

        try
        {
            contacts.Clear();
            foreach (var line in File.ReadAllLines(filename))
            {
                var parts = line.Split(',');
                if (parts.Length == 4)
                {
                    contacts.Add(new Contact
                    {
                        FirstName = parts[0],
                        LastName = parts[1],
                        Phone = parts[2],
                        Email = parts[3]
                    });
                }
            }
            changesMade = false;
            Console.WriteLine("Contacts loaded successfully.\n");
        }
        catch
        {
            Console.WriteLine("Error reading file.\n");
        }
    }

    static void StoreContacts()
    {
        Console.Write("### Store Contacts ###\nFilename: ");
        string filename = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(filename)) return;

        if (File.Exists(filename))
        {
            Console.Write($"WARNING: File \"{filename}\" already exists. Override? [Y/N] ");
            if (Console.ReadLine().Trim().ToUpper() != "Y") return;
        }

        try
        {
            File.WriteAllLines(filename, contacts.Select(c =>
                $"{c.FirstName},{c.LastName},{c.Phone},{c.Email}"));
            changesMade = false;
            Console.WriteLine("Contacts stored successfully.\n");
        }
        catch
        {
            Console.WriteLine("Error writing file.\n");
        }
    }

    static void ShowContacts()
    {
        Console.WriteLine("### Contact List ###");
        if (contacts.Count == 0)
        {
            Console.WriteLine("No contacts found.\n");
            return;
        }

        for (int i = 0; i < contacts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {contacts[i]}");
        }
        Console.WriteLine();
    }

    static void AddContact()
    {
        Console.WriteLine("### Add New Contact ###");
        Contact contact = new Contact();

        Console.Write("First Name: ");
        contact.FirstName = Console.ReadLine();

        Console.Write("Last Name: ");
        contact.LastName = Console.ReadLine();

        Console.Write("Phone: ");
        contact.Phone = Console.ReadLine();

        Console.Write("Email: ");
        contact.Email = Console.ReadLine();

        Console.WriteLine($"\nAdd this contact? [Y/N]\n{contact}");
        if (Console.ReadLine().Trim().ToUpper() == "Y")
        {
            contacts.Add(contact);
            changesMade = true;
            Console.WriteLine("Contact added.\n");
        }
        else
        {
            Console.WriteLine("Operation canceled.\n");
        }
    }

    static void EditContact()
    {
        ShowContacts();
        Console.Write("Select contact number to edit or leave blank to cancel: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > contacts.Count)
        {
            Console.WriteLine("Canceled or invalid index.\n");
            return;
        }

        Contact contact = contacts[index - 1];
        Console.WriteLine("Editing: " + contact);

        Console.Write("New First Name (leave blank to keep): ");
        string first = Console.ReadLine();
        Console.Write("New Last Name (leave blank to keep): ");
        string last = Console.ReadLine();
        Console.Write("New Phone (leave blank to keep): ");
        string phone = Console.ReadLine();
        Console.Write("New Email (leave blank to keep): ");
        string email = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(first)) contact.FirstName = first;
        if (!string.IsNullOrWhiteSpace(last)) contact.LastName = last;
        if (!string.IsNullOrWhiteSpace(phone)) contact.Phone = phone;
        if (!string.IsNullOrWhiteSpace(email)) contact.Email = email;

        changesMade = true;
        Console.WriteLine("Contact updated.\n");
    }

    static void DeleteContact()
    {
        ShowContacts();
        Console.Write("Select contact number to delete or leave blank to cancel: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > contacts.Count)
        {
            Console.WriteLine("Canceled or invalid index.\n");
            return;
        }

        Console.WriteLine($"Are you sure to delete: {contacts[index - 1]} [Y/N]");
        if (Console.ReadLine().Trim().ToUpper() == "Y")
        {
            contacts.RemoveAt(index - 1);
            changesMade = true;
            Console.WriteLine("Contact deleted.\n");
        }
        else
        {
            Console.WriteLine("Operation canceled.\n");
        }
    }

    static void MergeDuplicates()
    {
        Console.WriteLine("### Merge Duplicate Contacts ###");
        int mergedCount = 0;
        for (int i = 0; i < contacts.Count; i++)
        {
            for (int j = contacts.Count - 1; j > i; j--)
            {
                if (contacts[i].IsDuplicateOf(contacts[j]))
                {
                    Console.WriteLine($"Duplicate found:\n1. {contacts[i]}\n2. {contacts[j]}");
                    Console.Write("Keep [1] or [2]? ");
                    string choice = Console.ReadLine();
                    if (choice == "2") contacts[i] = contacts[j];
                    contacts.RemoveAt(j);
                    mergedCount++;
                    changesMade = true;
                }
            }
        }

        Console.WriteLine(mergedCount > 0 ? $"Merged {mergedCount} duplicates.\n" : "No duplicates found.\n");
    }

    static void ExitApplication()
    {
        if (changesMade)
        {
            Console.WriteLine("WARNING: You have made changes to the contact list that have not been stored.");
            Console.Write("Are you sure you want to exit? [Y/N] ");
            if (Console.ReadLine().Trim().ToUpper() != "Y")
            {
                Console.Clear();
                return;
            }
        }
        else
        {
            Console.Write("Are you sure you want to exit? [Y/N] ");
            if (Console.ReadLine().Trim().ToUpper() != "Y")
            {
                Console.Clear();
                return;
            }
        }

        Console.WriteLine("Thank you for using Contacts Book! Until next time!");
    }
}
