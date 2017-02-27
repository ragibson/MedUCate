using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApplication1
{
    using System.Xml;

    class Program
    {

        class QuestionObject
        {
            int _id;
            string _Question;
            string _Answer;
            string _FalseAnswer1;
            string _FalseAnswer2;
            string _FalseAnswer3;

            public QuestionObject(int id, string Question, string Answer, string FalseAnswer1, string FalseAnswer2, string FalseAnswer3)
            {
                this._id = id;
                this._Question = Question;
                this._FalseAnswer1 = FalseAnswer1;
                this._FalseAnswer2 = FalseAnswer2;
                this._FalseAnswer3 = FalseAnswer3;
                this._Answer = Answer;
            }

            public int Id { get { return _id; } }
            public string Question { get { return _Question; } }
            public string Answer { get { return _Answer; } }
            public string FalseAnswer1 { get { return _FalseAnswer1; } }
            public string FalseAnswer2 { get { return _FalseAnswer2; } }
            public string FalseAnswer3 { get { return _FalseAnswer3; } }
        }
        List<QuestionObject> MainQuestionSet = new List<QuestionObject>();
  
        static void writeQuestionSet(string DocName, List<QuestionObject> set)
        {
            using (XmlWriter writer = XmlWriter.Create(DocName))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("QuestionSet");

                foreach (QuestionObject question in set)
                {
                    writer.WriteStartElement("Question");

                    writer.WriteElementString("ID", question.Id.ToString());
                    writer.WriteElementString("Statement", question.Question);
                    writer.WriteElementString("Answer", question.Answer);
                    writer.WriteElementString("FalseAnswer1", question.FalseAnswer1);
                    writer.WriteElementString("FalseAnswer2", question.FalseAnswer2);
                    writer.WriteElementString("FalseAnswer3", question.FalseAnswer3);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        static void readQuestionSet(string DocName, List<QuestionObject> set)
        {
       
            XmlReader xmlReader = XmlReader.Create(DocName);
            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "Question"))
                {
                    if (xmlReader.HasAttributes)
                    {
                        int id = int.Parse(xmlReader.GetAttribute("ID"));
                        string statement = xmlReader.GetAttribute("Statement");
                        string answer = xmlReader.GetAttribute("Answer");
                        string falseAnswer1 = xmlReader.GetAttribute("FalseAnswer1");
                        string falseAnswer2 = xmlReader.GetAttribute("FalseAnswer2");
                        string falseAnswer3 = xmlReader.GetAttribute("FalseAnswer3");
                        QuestionObject addedobject = new QuestionObject(id,statement,answer,falseAnswer1,falseAnswer2,falseAnswer3);
                        set.Add(addedobject);
                    }
                }
            }
            Console.ReadKey();
        }
        static void writeTestQuestionSet()
        {
            List<QuestionObject> questionset = new List<QuestionObject>();        
            questionset.Add(new QuestionObject(1, "1 + 1?", "2", "1", "12", "2017"));
            questionset.Add(new QuestionObject(2, "2 + 2?", "4", "1", "12", "2017"));
            questionset.Add(new QuestionObject(3, "3 + 3?", "6", "1", "12", "2017"));
            questionset.Add(new QuestionObject(4, "4 + 4?", "8", "1", "12", "2017"));
            writeQuestionSet("TestQuestionSet.xml", questionset);
        }
        static void Main()
        {
            writeTestQuestionSet();
        }
    }
}
