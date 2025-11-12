; ModuleID = 'brainsharp'
source_filename = "brainsharp"

@tape = global [65535 x i8] zeroinitializer
@tape_idx = global i16 0
@fmt_num = private unnamed_addr constant [6 x i8] c"\0A$%d\0A\00", align 1
@fmt_char = private unnamed_addr constant [3 x i8] c"%c\00", align 1

declare i32 @printf(ptr, ...)

define i32 @main() {
entry:
  call void @move_right()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  br label %loop_cond

loop_cond:                                        ; preds = %loop_body, %entry
  %0 = load i16, ptr @tape_idx, align 2
  %1 = getelementptr [65535 x i8], ptr @tape, i16 0, i16 %0
  %2 = load i8, ptr %1, align 1
  %3 = icmp ne i8 %2, 0
  br i1 %3, label %loop_body, label %loop_exit

loop_body:                                        ; preds = %loop_cond
  call void @move_left()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @move_right()
  call void @sub()
  br label %loop_cond

loop_exit:                                        ; preds = %loop_cond
  call void @move_left()
  call void @print()
  call void @move_right()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  br label %loop_cond1

loop_cond1:                                       ; preds = %loop_body2, %loop_exit
  %4 = load i16, ptr @tape_idx, align 2
  %5 = getelementptr [65535 x i8], ptr @tape, i16 0, i16 %4
  %6 = load i8, ptr %5, align 1
  %7 = icmp ne i8 %6, 0
  br i1 %7, label %loop_body2, label %loop_exit3

loop_body2:                                       ; preds = %loop_cond1
  call void @move_left()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @move_right()
  call void @sub()
  br label %loop_cond1

loop_exit3:                                       ; preds = %loop_cond1
  call void @move_left()
  call void @add()
  call void @print()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @print()
  call void @print()
  call void @add()
  call void @add()
  call void @add()
  call void @print()
  call void @move_right()
  call void @move_right()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  br label %loop_cond4

loop_cond4:                                       ; preds = %loop_body5, %loop_exit3
  %8 = load i16, ptr @tape_idx, align 2
  %9 = getelementptr [65535 x i8], ptr @tape, i16 0, i16 %8
  %10 = load i8, ptr %9, align 1
  %11 = icmp ne i8 %10, 0
  br i1 %11, label %loop_body5, label %loop_exit6

loop_body5:                                       ; preds = %loop_cond4
  call void @move_left()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @move_right()
  call void @sub()
  br label %loop_cond4

loop_exit6:                                       ; preds = %loop_cond4
  call void @move_left()
  call void @add()
  call void @add()
  call void @print()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @print()
  call void @move_right()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  br label %loop_cond7

loop_cond7:                                       ; preds = %loop_body8, %loop_exit6
  %12 = load i16, ptr @tape_idx, align 2
  %13 = getelementptr [65535 x i8], ptr @tape, i16 0, i16 %12
  %14 = load i8, ptr %13, align 1
  %15 = icmp ne i8 %14, 0
  br i1 %15, label %loop_body8, label %loop_exit9

loop_body8:                                       ; preds = %loop_cond7
  call void @move_left()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @move_right()
  call void @sub()
  br label %loop_cond7

loop_exit9:                                       ; preds = %loop_cond7
  call void @move_left()
  call void @add()
  call void @print()
  call void @move_left()
  call void @print()
  call void @add()
  call void @add()
  call void @add()
  call void @print()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @print()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @sub()
  call void @print()
  call void @move_right()
  call void @move_right()
  call void @move_right()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  br label %loop_cond10

loop_cond10:                                      ; preds = %loop_body11, %loop_exit9
  %16 = load i16, ptr @tape_idx, align 2
  %17 = getelementptr [65535 x i8], ptr @tape, i16 0, i16 %16
  %18 = load i8, ptr %17, align 1
  %19 = icmp ne i8 %18, 0
  br i1 %19, label %loop_body11, label %loop_exit12

loop_body11:                                      ; preds = %loop_cond10
  call void @move_left()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @add()
  call void @move_right()
  call void @sub()
  br label %loop_cond10

loop_exit12:                                      ; preds = %loop_cond10
  call void @move_left()
  call void @add()
  call void @print()
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
