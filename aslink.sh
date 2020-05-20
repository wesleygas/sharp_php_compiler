nasm -f elf32 -F dwarf -g output.asm
ld -m elf_i386 -o output output.o
./output