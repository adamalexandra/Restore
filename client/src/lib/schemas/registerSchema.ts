import z from "zod";

const passwordValidation = new RegExp(
  /(?=^.{6,10}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\s).*$/
)//(between 6-10 characters)(at least one digit)(one lc)(one Uc)(one special character)

export const registerSchema = z.object({
  email: z.string(),
 password: z.string().regex(passwordValidation, {
    message: 'Password must contain 1 lowercase character, 1 uppercase character 1 number, 1 special and be 6-10 characters'
  })
});

export type RegisterSchema = z.infer<typeof registerSchema>;