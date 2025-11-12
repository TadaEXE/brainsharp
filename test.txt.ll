; ModuleID = 'brainsharp'
source_filename = "brainsharp"

@tape = global [65535 x i8] zeroinitializer
@tape_idx = global i16 0
@fmt_num = private unnamed_addr constant [6 x i8] c"\0A$%d\0A\00", align 1
@fmt_char = private unnamed_addr constant [3 x i8] c"%c\00", align 1

declare i32 @printf(ptr, ...)

define i32 @main() {
entry:
  %0 = load i16, ptr @tape_idx, align 2
  %1 = getelementptr i8, ptr @tape, i16 %0
  %2 = load i8, ptr %1, align 1
  %3 = call i32 (ptr, ...) @printf(ptr @fmt_char, i8 %2)
  %4 = load i16, ptr @tape_idx, align 2
  %5 = getelementptr [65535 x i8], ptr @tape, i16 %4
  %6 = load i8, ptr %5, align 1
  %7 = add i8 %6, 1
  store i8 %7, ptr %5, align 1
  %8 = load i16, ptr @tape_idx, align 2
  %9 = getelementptr i8, ptr @tape, i16 %8
  %10 = load i8, ptr %9, align 1
  %11 = call i32 (ptr, ...) @printf(ptr @fmt_char, i8 %10)
  %12 = load i16, ptr @tape_idx, align 2
  %13 = call i32 (ptr, ...) @printf(ptr @fmt_num, i16 %12)
  %14 = load i16, ptr @tape_idx, align 2
  %15 = add i16 %14, 1
  store i16 %15, ptr @tape_idx, align 2
  %16 = load i16, ptr @tape_idx, align 2
  %17 = getelementptr i8, ptr @tape, i16 %16
  %18 = load i8, ptr %17, align 1
  %19 = call i32 (ptr, ...) @printf(ptr @fmt_char, i8 %18)
  %20 = load i16, ptr @tape_idx, align 2
  %21 = getelementptr [65535 x i8], ptr @tape, i16 %20
  %22 = load i8, ptr %21, align 1
  %23 = sub i8 %22, 1
  store i8 %23, ptr %21, align 1
  %24 = load i16, ptr @tape_idx, align 2
  %25 = getelementptr i8, ptr @tape, i16 %24
  %26 = load i8, ptr %25, align 1
  %27 = call i32 (ptr, ...) @printf(ptr @fmt_char, i8 %26)
  %28 = load i16, ptr @tape_idx, align 2
  %29 = call i32 (ptr, ...) @printf(ptr @fmt_num, i16 %28)
  %30 = load i16, ptr @tape_idx, align 2
  %31 = sub i16 %30, 1
  store i16 %31, ptr @tape_idx, align 2
  %32 = load i16, ptr @tape_idx, align 2
  %33 = getelementptr i8, ptr @tape, i16 %32
  %34 = load i8, ptr %33, align 1
  %35 = call i32 (ptr, ...) @printf(ptr @fmt_char, i8 %34)
  %36 = load i16, ptr @tape_idx, align 2
  %37 = call i32 (ptr, ...) @printf(ptr @fmt_num, i16 %36)
  ret i32 0
}
