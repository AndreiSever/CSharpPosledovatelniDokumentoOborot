using System;
using System.Xml.Serialization;

namespace StudentAssignments.Web.Xml
{
    [XmlRoot("Шаблон")]
    public class WorkflowTemplate
    {
        [XmlArray("Документы")]
        [XmlArrayItem("Документ")]
        public Document[] Documents { get; set; }
        [XmlArray("Этапы")]
        [XmlArrayItem("Этап")]
        public Stage[] Stages { get; set; }
    }

    public class Document
    {
        [XmlAttribute(AttributeName = "Ид")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "Название")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "Тип")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "Обязательный")]
        public bool IsRequired { get; set; } = true;
    }

    public class Stage
    {
        [XmlAttribute(AttributeName = "Название")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "Срок")]
        public int Duration { get; set; }
        [XmlAttribute(AttributeName = "Возврат")]
        public bool CanReject { get; set; }
        [XmlArray("ВходныеДокументы")]
        [XmlArrayItem("Документ")]
        public InputDocument[] InputDocuments { get; set; }
        [XmlArray("ВыходныеДокументы")]
        [XmlArrayItem("Документ")]
        public OutputDocument[] OutputDocuments { get; set; }
    }

    public class InputDocument
    {
        [XmlAttribute(AttributeName = "Ид")]
        public string Id { get; set; }
    }

    public class OutputDocument
    {
        [XmlAttribute(AttributeName = "Ид")]
        public string Id { get; set; }
    }
}