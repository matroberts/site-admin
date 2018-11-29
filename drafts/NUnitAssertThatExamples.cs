using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class NUnitAssertThatExamples
{
    public class Newspaper
    {
        public string Name { get; set; }
        public DateTime PublicationDate { get; } = DateTime.UtcNow;
        public bool IsTabloid { get; set; }

        public string Download(string user)
        {
            switch (user)
            {
                case "Rupert":
                    var inner = new InvalidOperationException("User not found");
                    var outer = new ArgumentException("You need a subscription to download the newspaper", inner);
                    outer.Data.Add("username", user);
                    throw outer;
                default:
                    return Name;
            }
        }
    }

    [Test]
    public void CompareConstraintAndClassicModel()
    {
        var newspaper = new Newspaper(){Name = "The Times"};

        // Classic model of assert using Assert.AreEqual
        Assert.AreEqual("The Times", newspaper.Name);

        // Constraint model of assert using Assert.That
        Assert.That(newspaper.Name, Is.EqualTo("The Times"));
    }

    [Test]
    public void TestStrings()
    {
        var sentence = "The good, the bad and the ugly";

        Assert.That(sentence, Is.Not.Null.Or.Empty);
        Assert.That(sentence, Is.EqualTo("The good, the bad and the ugly"));

        Assert.That(sentence, Does.StartWith("The good"));
        Assert.That(sentence, Does.Contain("the bad"));
        Assert.That(sentence, Does.EndWith("The ugly").IgnoreCase);

        Assert.That(sentence, Contains.Substring("the bad"));
    }

    [Test]
    public void TestExceptions()
    {
        var newspaper = new Newspaper();

        // Assert that exception not thrown
        Assert.That(() => newspaper.Download("Bobby"), Throws.Nothing);

        // Assert that exception is thrown
        Assert.That(() => newspaper.Download("Rupert"), Throws.Exception.TypeOf<ArgumentException>()
                                                              .With.Message.EqualTo("You need a subscription to download the newspaper"));
        // Shorter constraints for common exception types
        Assert.That(() => newspaper.Download("Rupert"), Throws.ArgumentException);
        // Inner exceptions
        Assert.That(() => newspaper.Download("Rupert"), Throws.ArgumentException.And.InnerException.Message.EqualTo("User not found")
                                                                                .And.InnerException.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void TestDates()
    {
        var start = DateTime.UtcNow;

        var newspaper = new Newspaper();
        
        Assert.That(newspaper.PublicationDate, Is.GreaterThanOrEqualTo(start));
        // DateTime and Timespan Assertions can contain a tolerance
        Assert.That(newspaper.PublicationDate, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
    }

    [Test]
    public void TestCollections()
    {
        var numbers = new List<int>(){ 2, 4, 6, 8 };

        // Equality => element by element equality
        Assert.That(numbers, Is.EqualTo(new []{ 2, 4, 6, 8 }));

        // Equivalence => same elements, but can be in a different order
        Assert.That(numbers, Is.EquivalentTo(new []{ 8, 6, 4, 2 }));

        // Counts
        Assert.That(numbers.Count, Is.EqualTo(4));
        Assert.That(numbers, Has.Count.EqualTo(4));

        // Empty collection
        Assert.That(new List<int>(), Is.Empty);
    }

    [Test]
    public void MoreCollections()
    {
        var newspapers = new List<Newspaper> { new Newspaper(){Name = "The Times", IsTabloid = false}, new Newspaper(){Name = "The Sun", IsTabloid = true}, new Newspaper(){Name = "The Mirror", IsTabloid = true }};

        // Has can be followed by a "counting" constraint
        // Matches allows you to write a lambda for the constraint
        Assert.That(newspapers, Has.Some.Matches<Newspaper>(n => n.Name.Contains("s")));
        Assert.That(newspapers, Has.One.Matches<Newspaper>(n => n.IsTabloid == false));
        Assert.That(newspapers, Has.Exactly(2).Matches<Newspaper>(n => n.IsTabloid));
        Assert.That(newspapers, Has.All.Matches<Newspaper>(n => n.Name.StartsWith("The")));
        Assert.That(newspapers, Has.None.Matches<Newspaper>(n => n.Name == "The Post"));
    }


}
