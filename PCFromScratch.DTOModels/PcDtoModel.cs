namespace PCFromScratch.DTOModels;

public record struct PcDtoModel(Guid? Cpu, Guid? Motherboard, Guid? Gpu, Guid? Ram, Guid? Cooler,
    List<Guid> InternalDrives, Guid? Psu);
