using Server;
using System;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Services.TownCryer
{
    public class CreateGreetingEntryGump : BaseTownCryerGump
    {
        public TownCryerGreetingEntry Entry { get; set; }
        public bool Edit { get; private set; }

        public CreateGreetingEntryGump(PlayerMobile pm, TownCrier cryer, TownCryerGreetingEntry entry = null)
            : base(pm, cryer)
        {
            Entry = entry;

            if (Entry != null)
            {
                Edit = true;
            }
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            AddHtmlLocalized(58, 140, 100, 20, 1158027, false, false); // Author:
            AddLabel(105, 140, 0, String.Format("{1} {0}", User.Name, User.AccessLevel.ToString()));

            AddHtmlLocalized(58, 160, 100, 20, 1158026, false, false); // Headline:
            AddBackground(58, 180, 740, 20, 0x2486);
            AddTextEntry(59, 180, 739, 20, 0, 1, Entry != null ? Entry.Title.ToString() : _Headline);

            AddHtmlLocalized(58, 200, 120, 20, 1158028, false, false); // Body Paragraph 1:
            AddBackground(58, 220, 740, 40, 0x2486);
            AddTextEntry(59, 220, 739, 40, 0, 2, _Body);

            AddHtmlLocalized(58, 270, 120, 20, 1158029, false, false); // Body Paragraph 2:
            AddBackground(58, 290, 740, 40, 0x2486);
            AddTextEntry(59, 290, 739, 40, 0, 3, _Body2);

            AddHtmlLocalized(58, 340, 120, 20, 1158030, false, false); // Body Paragraph 3:
            AddBackground(58, 360, 740, 40, 0x2486);
            AddTextEntry(59, 360, 739, 40, 0, 4, _Body3);

            AddHtml(58, 410, 250, 20, "Link:", false, false);
            AddBackground(58, 430, 740, 40, 0x2486);
            AddTextEntry(59, 430, 739, 40, 0, 5, _Link);

            AddHtml(58, 480, 250, 20, "Link Text:", false, false);
            AddBackground(58, 500, 740, 40, 0x2486);
            AddTextEntry(59, 500, 739, 40, 0, 6, _LinkText);

            AddBackground(155, 550, 20, 20, 0x2486);
            AddHtmlLocalized(58, 550, 100, 20, 1158031, false, false); // Expiry (in days):
            AddTextEntry(156, 550, 19, 20, 0, 7, _Expires);

            AddButton(40, 615, 0x601, 0x602, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(63, 615, 150, 20, 1077787, false, false); // Submit
        }

        private void HandleText(RelayInfo info)
        {
            TextRelay relay = info.GetTextEntry(1);

            if (relay != null && !string.IsNullOrEmpty(relay.Text))
            {
                _Headline = relay.Text.Trim();
            }

            relay = info.GetTextEntry(2);

            if (relay != null && !string.IsNullOrEmpty(relay.Text))
            {
                _Body = relay.Text.Trim();
            }

            relay = info.GetTextEntry(3);

            if (relay != null && !string.IsNullOrEmpty(relay.Text))
            {
                _Body2 = relay.Text.Trim();
            }

            relay = info.GetTextEntry(4);

            if (relay != null && !string.IsNullOrEmpty(relay.Text))
            {
                _Body3 = relay.Text.Trim();
            }

            relay = info.GetTextEntry(5);

            if (relay != null && !string.IsNullOrEmpty(relay.Text))
            {
                _Link = relay.Text.Trim();
            }

            relay = info.GetTextEntry(6);

            if (relay != null && !string.IsNullOrEmpty(relay.Text))
            {
                _LinkText = relay.Text.Trim();
            }

            relay = info.GetTextEntry(7);

            if (relay != null && !string.IsNullOrEmpty(relay.Text))
            {
                _Expires = relay.Text.Trim();
            }
        }

        private string _Headline;
        private string _Body;
        private string _Body2;
        private string _Body3;
        private string _Expires;
        private string _Link;
        private string _LinkText;

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                HandleText(info);

                var headline = _Headline;
                var body = _Body;
                var body2 = _Body2;
                var body3 = _Body3;
                var exp = _Expires;
                var link = _Link;
                var linkText = _LinkText;

                int expires = Utility.ToInt32(exp);

                if (!String.IsNullOrEmpty(body))
                {
                    if (!String.IsNullOrEmpty(body2))
                        body += body2;

                    if (!String.IsNullOrEmpty(body3))
                        body += body3;
                }

                if (Entry == null)
                {
                    Entry = new TownCryerGreetingEntry(headline, body, expires, link, linkText);
                }
                else
                {
                    Entry.Title = headline;
                    Entry.Body = body;
                    Entry.Link = link;
                    Entry.LinkText = linkText;

                    if (expires >= 1 && expires <= 30)
                    {
                        Entry.Expires = DateTime.Now + TimeSpan.FromDays(expires);
                    }
                }

                if(expires < 1 || expires > 30)
                {
                    User.SendLocalizedMessage(1158033); // The expiry can be between 1 and 30 days. Please check your entry and try again.
                }
                else if (string.IsNullOrEmpty(headline) || string.IsNullOrEmpty(body) || headline.Length < 5 || body.Length < 5)
                {
                    User.SendLocalizedMessage(1158032); // You have made an illegal entry.  Check your entries and try again.
                }
                else
                {
                    if (!Edit)
                    {
                        TownCryerSystem.AddEntry(Entry);
                    }

                    User.SendLocalizedMessage(1158039); // Your entry has been submitted.

                    return;
                }

                Refresh();
            }
        }
    }
}