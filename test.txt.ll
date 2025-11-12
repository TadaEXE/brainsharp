; ModuleID = 'brainsharp'
source_filename = "brainsharp"

@tape = global [65535 x i8] zeroinitializer
@tape_idx = global i16 0
@fmt_num = private unnamed_addr constant [6 x i8] c"\0A$%d\0A\00", align 1
@fmt_char = private unnamed_addr constant [3 x i8] c"%c\00", align 1

declare i32 @printf(ptr, ...)

define i32 @main() {
entry:
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @move_right()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @move_right()
  call void @add()
  call void @move_right()
  call void @add()
  call void @move_right()
  call void @sub()
  call void @move_right()
  call void @move_right()
  call void @add()
  ret i32 0
}

define void @get_pos() {
entry:
  %0 = load i16, ptr @tape_idx, align 2
  %1 = call i32 (ptr, ...) @printf(ptr @fmt_num, i16 %0)
  ret void
}

define void @print() {
entry:
  %0 = load i16, ptr @tape_idx, align 2
  %1 = getelementptr [65535 x i8], ptr @tape, i16 0, i16 %0
  %2 = load i8, ptr %1, align 1
  %3 = call i32 (ptr, ...) @printf(ptr @fmt_char, i8 %2)
  ret void
}

define void @add() {
entry:
  %0 = load i16, ptr @tape_idx, align 2
  %1 = getelementptr [65535 x i8], ptr @tape, i16 0, i16 %0
  %2 = load i8, ptr %1, align 1
  %3 = add i8 %2, 1
  store i8 %3, ptr %1, align 1
  ret void
}

define void @sub() {
entry:
  %0 = load i16, ptr @tape_idx, align 2
  %1 = getelementptr [65535 x i8], ptr @tape, i16 0, i16 %0
  %2 = load i8, ptr %1, align 1
  %3 = sub i8 %2, 1
  store i8 %3, ptr %1, align 1
  ret void
}

define void @move_right() {
entry:
  %0 = load i16, ptr @tape_idx, align 2
  %1 = add i16 %0, 1
  store i16 %1, ptr @tape_idx, align 2
  ret void
}

define void @move_left() {
entry:
  %0 = load i16, ptr @tape_idx, align 2
  %1 = sub i16 %0, 1
  store i16 %1, ptr @tape_idx, align 2
  ret void
}
