using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogSystem : MonoBehaviour
{
    public class Check
    {
        public Check(string valueName, int valueMin)
        {
            m_valueMin = valueMin;
            m_valueName = valueName;
        }

        public string m_valueName;
        public int m_valueMin;
    }

    public class DialogPair
    {
        public DialogPair(List<Check> requirements, string dialog)
        {
            m_requirements = requirements;
            m_dialog = dialog;
        }

        public List<Check> m_requirements;
        public string m_dialog;
    }

    public class DialogLine
    {
        public DialogLine(List<Check> modifiers, List<DialogPair> lines, List<DialogPair> responses)
        {
            m_modifiers = modifiers;
            m_lines = lines;
            m_responses = responses;
        }

        public List<Check> m_modifiers;
        public List<DialogPair> m_lines;
        public List<DialogPair> m_responses;
    }


    public class DialogScene
    {
        Dictionary<string, DialogLine> m_lines;

        public DialogScene(Dictionary<string, DialogLine> lines)
        {
            m_lines = lines;
        }

        public void ApplyModifiers(string line)
        {
            foreach (Check modifier in m_lines[line].m_modifiers)
            {
                PlayerPrefs.SetInt(modifier.m_valueName, PlayerPrefs.GetInt(modifier.m_valueName,0) + modifier.m_valueMin);
            }
        }

        public List<string> GetDialog(string line)
        {
            ApplyModifiers(line);

            List<string> text = new List<string>();
            text.Add("");
            foreach (DialogPair l in m_lines[line].m_lines)
            {
                bool shouldContinue = false;
                foreach (Check c in l.m_requirements)
                {
                    if (PlayerPrefs.GetInt(c.m_valueName) <= c.m_valueMin)
                    {
                        shouldContinue = true;
                    }
                }

                if (shouldContinue)
                {
                    continue;
                }

                text[0] += l.m_dialog;
            }

            text.Add("");
            text.Add("");

            foreach (DialogPair l in m_lines[line].m_responses)
            {
                bool shouldContinue = false;
                foreach (Check c in l.m_requirements)
                {
                    if (PlayerPrefs.GetInt(c.m_valueName) < c.m_valueMin)
                    {
                        shouldContinue = true;
                    }
                }

                if (shouldContinue)
                {
                    continue;
                }
                text[2] = text[1];
                text[1] = l.m_dialog;

            }
            return text;
        }
    }

    public class DialogLevel
    {
        public DialogScene m_beforeCharacterCustomization;
        public DialogScene m_beforeMainLevel;
        public DialogScene m_beforeMainTournament;
        public DialogScene m_beforeFailure;
        public DialogScene m_beforeSuccess;

        public DialogLevel(DialogScene BeforeCharacterCustomization, DialogScene BeforeMainLevelCustomization,
            DialogScene BeforeMainTournament, DialogScene BeforeFailure, DialogScene BeforeSuccess)
        {
            m_beforeCharacterCustomization = BeforeCharacterCustomization;
            m_beforeMainLevel = BeforeMainLevelCustomization;
            m_beforeMainTournament = BeforeMainTournament;
            m_beforeFailure = BeforeFailure;
            m_beforeSuccess = BeforeSuccess;
        }


        public string m_startingLine;
    }


    private List<DialogLevel> m_dialogExchanges;
    [SerializeField]
    private Text m_dialogText;
    [SerializeField]
    private UnityEngine.UI.Button m_buttonA;
    [SerializeField]
    private UnityEngine.UI.Button m_buttonB;
    [SerializeField]
    private Text m_choiceA;
    [SerializeField]
    private Text m_choiceB;

    [SerializeField]
    private TextAsset m_levelText;

    [SerializeField]
    private UnityEngine.UI.Image m_buttonImageB;

    [SerializeField]
    private AudioSource m_source;

    [SerializeField]
    private AudioClip m_clip;

    private static DialogSystem ms_instance;
    private static string m_playerChoice;

    public void LoadScenes()
    {
        ms_instance = this;
        m_dialogExchanges = new List<DialogLevel>();
        LoadLevelOne();

    }

    public static void SetChoice(string choice)
    {
        ms_instance.m_source.PlayOneShot(ms_instance.m_clip);
        m_playerChoice = choice;
        
        ms_instance.m_buttonA.interactable = false;
        ms_instance.m_buttonB.interactable = false;

        ms_instance.m_choiceA.text = "";
        ms_instance.m_choiceB.text = "";
    }

    public void LoadLevelOne()
    {
        Dictionary<string, DialogLine> beforeCharacterCustomizationDictionary = new Dictionary<string, DialogLine>();

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "start";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Practicality", 1));
            modifiers.Add(new Check("Creativity", 1));
            modifiers.Add(new Check("Intelligence", 1));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Click the \"Start Game\" button to skip this dialog."));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "show me the dialog")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Start game"));
            }



            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "show me the dialog";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Practicality", 1));
            modifiers.Add(new Check("Creativity", 1));
            modifiers.Add(new Check("Intelligence", 1));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them: \"Hello prospective employee #(generate random number here). Thank you for applying to the Monstinder training program!"));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "We wish you the best of luck in your route to becoming a professional 'Monsta-Matcha', as us humourous, hip folks back at the council like to call it. "));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "In all seriousness however, Monster Matchers are in high demand now due to Monsteria's *underpopulation crisis*. "));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "In the real world, mismatched pairs lead to unhappy couples, which then lead to bad marriages, consistent arguing, traumatized children, and legal hassles over one partner being uncooperative during the divorce procedures...that means you Sasha. "));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "To begin training, we shall determine your aptitude and general knowledge of the state of Monsteria through some breif questioning. What is your assumed reasoning behind this mortal?"));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Do you see those polaroid shaped buttons with words on them. Click on one of them to respond to me.\n"));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Monsters Be Ugly...")); //written
            }

            {
                checks = new List<Check>();
                checks.Add(new Check("Autism", 1));
                checks.Add(new Check("Intelligence", 1));
                responses.Add(new DialogPair(checks, "Idk, they're asexual?"));
            }



            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Idk, they're asexual?";
            List<Check> modifiers = new List<Check>();;
            modifiers.Add(new Check("Intelligence", 5));
            modifiers.Add(new Check("Autism", 10));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"Um, monsters are asexual?\""));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "\nThem: \"Well, no, care for another guess?\""));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();
            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Monsters Be Ugly...")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Just take me to the game.")); //written
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Just take me to the game.";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Practicality", 1));
            modifiers.Add(new Check("Creativity", 1));
            modifiers.Add(new Check("Intelligence", 1));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "\nThem: \"Look just say tutorial if you want to get started\""));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();
            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "")); //written
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Monsters Be Ugly...";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Practicality", 1));
            modifiers.Add(new Check("Intelligence", 4));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"Monsters are too ugly & incompatible to fall in love with one another, and because love is needed to call the Monsterodactyl to deliver the monster babies from the gods, no babies are being delivered\""));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them: \"Uhm, well yeah I guess that's kinda right. Monsteria's button-mashing monster populace have many different elemental types, such as fire, ice, water, earth, and Sasha -erhm- I mean spirit. In this simulation, you're tasked with curating each Monstinder user's selection of available matches by determining whether or not they are compatible.\""));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Why")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Sounds like Tedious"));
            }
            
            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Why";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Practicality", 6));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them: \"Remember that the app's goal isn't to help monsters find meaningful relationships. Most monsters don't even understand who they're attracted to, let alone how to ensure a successful relationship with a prospective partner. \""));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Seems a bit superficial to me")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }



        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Seems a bit superficial to me";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Practicality", 10));
            modifiers.Add(new Check("Intelligence", 10));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them: \"Huh, now that you mention it, I'm sure that it wouldn't be too difficult to integrate psychological tests to evaluate a Monster's 'personality'- meaning their means of interacting & communicating with others, approach to solving conflict, and ways of having fun. We should probably table that for now and implement that as a last resort if this app still doesn't help increase Monsteria's population. Always remember, time is money. So greater efficiency equals more money, and more money equals more happiness. More happiness means more sex, and more sex means more Monsters. Henceforward therefore, in the firm conviction that my monster-fraternity hazes me, I conclude that monsters, money, and sex are all time \""));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "that was a bit of a rant")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial"));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }


        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "that was a bit of a rant";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Practicality", 10));
            modifiers.Add(new Check("Intelligence", 10));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"can we just get on with the training already? Jeez I didn't ask for this much world-building. Ugh, and stop being so meta all the time, it's getting old! Ew, by continuing this piece of dialogue, I'm getting even more meta. \""));
                }
            }
            {
                {
                    checks = new List<Check>();
                    checks.Add(new Check("Autism", 3));
                    lines.Add(new DialogPair(checks, "Don't hipster out your prospective employees with this artsy-fartsy trying-too-hard-to-be-original-ands-disguise-simple-game-mechanics-which-aren't-too-fun-with-weird-dialogue nonsense\""));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Sounds like Tedious";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Fun", 3));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"Sounds like Tedious bureacratic grunt work only suitable for a corporate robot."));

                }
                {
                    checks = new List<Check>();
                    checks.Add(new Check("Creativity", 3));
                    lines.Add(new DialogPair(checks, "Unlike me: a highly creative, unique visionary capable of greatness!"));

                }
                {
                    checks = new List<Check>();
                    checks.Add(new Check("Attention to detail", 1));
                    lines.Add(new DialogPair(checks, "I can't wait to get started.\""));

                }
                {
                    checks = new List<Check>();;
                    lines.Add(new DialogPair(checks, "\nThem: \"Hey I mean it was your decision to take up the training for this job, not mine.  You can quit at any time you want buddy. "));

                }
                {
                    checks = new List<Check>(); ;
                    checks.Add(new Check("Addiction",1));
                    checks.Add(new Check("Attention to lore", 1));
                    lines.Add(new DialogPair(checks, "But I know you won't, you can't. You're just too deep in already. You're too interested in the lore of Monsteria - lore which will offer profound insights into the complexities of Monster-life.\n"));

                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Just take me to the game.")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "tutorial";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    checks.Add(new Check("Fun", 1));
                    lines.Add(new DialogPair(checks, "Them: \"Now let's do the monster Mash!"));
                }
                {
                    checks = new List<Check>();
                    checks.Add(new Check("Fun", 1));
                    checks.Add(new Check("Autism", 1));
                    lines.Add(new DialogPair(checks, "Okay let's get serious now. "));

                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "All that matters in determining a Monster's compatibility with another is their physical, or elemental compatibility. What's important is their ability to get down and dirty if ya know what I'm sayin. It should be quite obvious that the intermingling of genitalia between a fire monster and ice monster would be more painful than pleasureable.\""));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "But is it?")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Huh,"));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Huh,";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them: \"Have any monsters with opposing elements tried to do it before?"));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Yep. It's led to alot of shock videos on the monster-net, and many edits in legislation. You'd think these people would have common sense, but no - no they don't. So many people don't. It's unfortunate the opposites attract, beacause you can't be that different from your lover you know? You gotta be at least fundamentallt the same, I believe. But true love isn't what matters right now, sex is. Sex is all that matters. "));

                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "All that matters in determining a Monster's compatibility with another is their physical, or elemental compatibility. What's important is their ability to get down and dirty if ya know what I'm sayin. It should be quite obvious that the intermingling of genitalia between a fire monster and ice monster would be more painful than pleasureable.\""));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "just like the philosophy of humans")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "the other choice has bean jokes"));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }



        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "just like the philosophy of humans";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"One can even observe similarities between the sexually charged ambitions of a human and those of a human bean. fun fact: Human bean plants are the most sexual kind of bean plant. They really can't control themselves, or keep their friend's secrets - someone always winds up spillin' the beans. They also bald quite easily, and not even beanies can cover up the bald spots. They do like watching Mr.Bean, however and consider him to be their god from which they all descended from. Monsteria may be a weird world, but nowhere near as weird as human beans and beings. Gosh, I love saying beans right now. I could just say beans all day if I wanted to. Try it. Try saying beans. Do it."));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial two")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }



        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "the other choice has bean jokes";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"just pick that one okay. Stop trying to be a rebel. Okay, here. You punishment is not recieving any self-aware, off-beat, quirky, absurdist, humour. Unless you consider this to be funny, which it isn't. If you found it to be funny, please learn to consume better media you unsophisticated, uncultured human bean. Go talk to some hipster bloggers at your local coffee beanery, and start eating string beans. Beans. Gosh, I love beans."));
                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial two")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "But is it?";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Kinkiness", 5));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"But is it? Or is it the kind of sensation that hurts so good. Ohhh yeah bae-bayyyy.\""));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them:\"Um, idk. Maybe if you're into that kinda thing. It's a bit sick though, but I guess it's also kinky. Hey, out of curiosity, how kinky would you say you are? Would you place a slice of honeydew on your lover's foot and then eat it while intently looking at his/her/its eye/eyeball/eye-socket/telepathic-eye-thing-that-monsters-have-that-I'm-totally-not-making-up-on-the-spot. \""));

                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Absolutely")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "excuse me"));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }



        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Absolutely";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Kinkiness", 5));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"**Absolutely** That's kinky. Any back in my younger days actually, I was quite renown within the underground community for my skills with honeydew and beans. Humans beans specifically."));
                }
                {
                    checks = new List<Check>();
                    checks.Add(new Check("Intelligence", 4));
                    lines.Add(new DialogPair(checks, " Human beans are quite fiesty and very sexually motivated\""));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them:\"Yeah humans beans are the most erotic kind of means. They live to breed. No wonder we have such a surplus of human beans. Whenever I go to Monsta-potle for a monstaritto, they're always out of pinto beans - but they never run out of human beans. Honestly, I'm quite sick of human beans. Variety is spice of life as much as it is for monstarittos. Now, shall we move on with the tutorial and actually get on with the training? \""));

                }
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "excuse me";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Kinkiness", 10));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"excuse me are you trying to flirt with me? I think you may just be a little bit too fast, and a little bit too flirtatious. You kinky monsta you ;)\""));
                }

            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Whipped cream, cherries, handcuffs, chains")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "boisonberry pie, disney movies"));
            }

            {
                checks = new List<Check>();
                checks.Add(new Check("Autism", 10));
                responses.Add(new DialogPair(checks, "chocolate handcuffs"));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }


        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Whipped cream, cherries, handcuffs, chains";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them: \"Ok this as gotten out of hand. We need to get to work so just say tutorial.\""));
                }

            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial two")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }
        

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "boisonberry pie, disney movies";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them: \"Ok this as gotten out of hand. We need to get to work so just say tutorial.\""));
                }

            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial two")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "chocolate handcuffs";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them: \"Ok this as gotten out of hand. We need to get to work so just say tutorial two.\""));
                }

            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial two")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }


        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "tutorial two";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "Them: \"Oh right. So I'm supposed to explain the simulation. First, assemble your monster with the different monsta-bodyparts. To add complexity to the simulation, we took snapshots of our stripped prisoners - I mean volunteers' components and created an algorithm to merge the body parts of these monsters into even more elementally-diverse monsterllings, who all enjoy a refreshing can of Monster."));
                }

                {
                    checks = new List<Check>();
                    checks.Add(new Check("Rediculousness",1));
                    checks.Add(new Check("Meta", 1));
                    lines.Add(new DialogPair(checks, " Our programmers somehow took extra care to avoid pesky merge conflicts in Git.Hey, nobody wants you here Git.Git out here.Git good or git gone."));
                }

                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, " Monster, get yours today. Type in keyboardkommander for a 0 percent discount and your next purchase. 100% of the nonexistent proceeds will go to the I'm-a-flamboyantly-autistic-walking-stereotype-of-a-game-developer-and-I'm - proud organization.\""));
                }

            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Okay Whatever")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }


        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Okay Whatever";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Sass", 3));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "You: \"Get back to explaining the game to me...wierdo?\""));
                }
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "\nThem: \"Just say tutorial three to continue\""));
                }

            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "tutorial three")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "tutorial three";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "\nThem: \"On with the explanation. To keep this simple, many prospective monsta-matchers grew too confused with the overwhelming biodiversity of the Monster species. So, for the first 'lovel' (heh you like dat pun bb~) of our training simulation, you match similar element types with one another. After you've built your monster, it's time to find it some good matches.'\""));
                }

            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "well how")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "well how";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "\nYou: \"Well how do you do that.\""));
                }

                {
                    checks = new List<Check>();
                    lines.Add(new DialogPair(checks, "\nThem: \"Monsters match when they have similar elements. There are only two elements on this lovel, fire and water, so it should be easy enough for you to work out. Remember dum - dum, hit the match button when you think it's a match, and the no button when you think it\'s not.Just remember, No for no match, and match for match.Okay? Both \'No Match\' and \'No\' start with the letter \"N\", so it is an easy pneumonic.Hit the button that says end to get started. Just say end to get started\""));
                }

            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Start game")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, ""));
            }

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        DialogScene beforeCharacterCustomization = new DialogScene(beforeCharacterCustomizationDictionary);

        m_dialogExchanges.Add(new DialogLevel(beforeCharacterCustomization, beforeCharacterCustomization, beforeCharacterCustomization, beforeCharacterCustomization, beforeCharacterCustomization));

    }

    private IEnumerator RunScene(DialogScene scene, string startingLine = "start")
    {
        m_dialogText.text = "";
        m_choiceA.text = "";
        m_choiceB.text = "";
        m_dialogText.text = "";
        m_playerChoice = "start";
        Fader.Instance.FadeOut(m_dialogText.gameObject);

        bool first = true;
        while (m_playerChoice != "Start game")
        {
            List<string> text = null;
            try {
                text = scene.GetDialog(m_playerChoice);
            }
            catch
            {
                Debug.LogError(m_playerChoice);
            }

            if (!first)
            {
                Fader.Instance.FadeIn(m_dialogText.gameObject);
            }
            yield return new WaitForSeconds(1.0f);
            m_dialogText.text = text[0];
            m_choiceA.text = text[1];
            m_choiceB.text = text[2];

            if(m_choiceB.text == "")
            {
                m_buttonImageB.color = Color.clear;
            }
            else
            {
                m_buttonImageB.color = Color.white;

            }
            if (!first)
            {
                Fader.Instance.FadeOut(m_dialogText.gameObject);
            }
            first = false;

            m_playerChoice = "";

            if (m_choiceA.text != "")
            {
                m_buttonB.interactable = true;
            }
            if (m_choiceB.text != "")
            {
                m_buttonA.interactable = true;
            }
            while (m_playerChoice.Length == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(1.0f);
        }


    }

    // Use this for initialization
    void Start()
    {
        m_choiceA.color = Color.white;
        m_choiceB.color = Color.white;
        StartCoroutine(RunLevel());
    }

    private IEnumerator RunLevel()
    {
        LoadScenes();
        int currentLevel = PlayerPrefs.GetInt("Level", 0);
        m_dialogText.text = "";

        if (currentLevel >= m_dialogExchanges.Count)
        {
            currentLevel = m_dialogExchanges.Count - 1;
        }

        switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            case "DialogBeforeCharacterCustomization":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeCharacterCustomization);
                while (m_playerChoice != "Start game") { yield return new WaitForEndOfFrame(); }


                Fader.Instance.FadeIn().LoadLevel("CharacterCustomization").FadeOut();
                break;
            case "DialogBeforeMainLevel":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeMainLevel);
                while (m_playerChoice != "Start game") { yield return new WaitForEndOfFrame(); }
                Fader.Instance.FadeIn().LoadLevel("PrototypeScene").FadeOut();
                break;
            case "DialogBeforeMainTournament":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeMainTournament);
                while (m_playerChoice != "Start game") { yield return new WaitForEndOfFrame(); }
                Fader.Instance.FadeIn().LoadLevel("MatchRejects").FadeOut();
                break;
            case "DialogBeforeFailure":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeFailure);
                while (m_playerChoice != "Start game") { yield return new WaitForEndOfFrame(); }
                Fader.Instance.FadeIn().LoadLevel("Failure").FadeOut();
                break;
            case "DialogBeforeSuccess":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeSuccess);
                while (m_playerChoice != "Start game") { yield return new WaitForEndOfFrame(); }
                Fader.Instance.FadeIn().LoadLevel("Success").FadeOut();
                break;
        }


    }
}
