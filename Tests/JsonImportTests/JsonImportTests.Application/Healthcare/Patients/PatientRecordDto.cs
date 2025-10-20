using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Healthcare.Patients
{
    public class PatientRecordDto
    {
        public PatientRecordDto()
        {
            RecordType = null!;
            Patient = null!;
            Practitioner = null!;
            Diagnosis = null!;
            Prescriptions = null!;
            VitalSigns = null!;
        }

        public Guid RecordId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; }
        public PatientDto Patient { get; set; }
        public PractitionerDto Practitioner { get; set; }
        public DiagnosisDto Diagnosis { get; set; }
        public List<PrescriptionDto> Prescriptions { get; set; }
        public VitalSignDto VitalSigns { get; set; }

        public static PatientRecordDto Create(
            Guid recordId,
            Guid patientId,
            DateTime recordDate,
            string recordType,
            PatientDto patient,
            PractitionerDto practitioner,
            DiagnosisDto diagnosis,
            List<PrescriptionDto> prescriptions,
            VitalSignDto vitalSigns)
        {
            return new PatientRecordDto
            {
                RecordId = recordId,
                PatientId = patientId,
                RecordDate = recordDate,
                RecordType = recordType,
                Patient = patient,
                Practitioner = practitioner,
                Diagnosis = diagnosis,
                Prescriptions = prescriptions,
                VitalSigns = vitalSigns
            };
        }
    }
}